using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum CUSTOMER_STATE
{
    IDLE_IN_SPAWNER,    //스포너 대기
    IDLE_IN_DISPLAY,    //진열대 대기
    IDLE_IN_CASHIER,    //계산대 대기
    MOVE_TO_DISPLAY,    //빵 진열대로 이동
    MOVE_TO_CASHIER,    //계산대로 이동 (1.포장, 2.홀식사)
    MOVE_TO_HALL,       //홀로 이동
    EAT_IN_HALL,        //홀에서 식사
    TOGO                //포장해서 나감
}

public abstract class CustomerBaseState
{
    protected CustomerController customerController { get; private set; }
    public CustomerBaseState(CustomerController cc)
    {
        this.customerController = cc;
    }

    //상태 패턴

    public abstract void OnEnterState();
    public abstract void OnUpdateState();
    public abstract void OnFixedUpdateState();
    public abstract void OnExitState();


}

public class CustomerStatePattern
{
    public CustomerBaseState currentBaseState { get; private set; }
    private Dictionary<CUSTOMER_STATE, CustomerBaseState> dicState = new Dictionary<CUSTOMER_STATE, CustomerBaseState>();

    public CustomerStatePattern(CUSTOMER_STATE enumState, CustomerBaseState customerBaseState)
    {
        AddState(enumState, customerBaseState);
        currentBaseState = GetState(enumState);
    }

    public void AddState(CUSTOMER_STATE enumState, CustomerBaseState customerBaseState)
    {
        if (!dicState.ContainsKey(enumState))
        {
            dicState.Add(enumState, customerBaseState);
        }
    }

    public CustomerBaseState GetState(CUSTOMER_STATE enumState)
    {
        if (dicState.TryGetValue(enumState, out CustomerBaseState state))
        {
            return state;
        }

        return null;
    }

    public void DeleteState(CUSTOMER_STATE enumState)
    {
        if (dicState.ContainsKey(enumState))
        {
            dicState.Remove(enumState);
        }
    }

    public void ChangeState(CUSTOMER_STATE enumState)
    {
        currentBaseState?.OnExitState();

        if (dicState.TryGetValue(enumState, out CustomerBaseState state))
        {
            currentBaseState = state;
        }

        currentBaseState?.OnEnterState();
    }

    public void UpdateState()
    {
        currentBaseState?.OnUpdateState();
    }

    public void FixedUpdateState()
    {
        currentBaseState?.OnFixedUpdateState();
    }
}


public class CustomerController : MonoBehaviour
{
    public NavMeshAgent agent;
    //public Animator animator;         //애니메이터 제거
    private Transform canvas;
    public int myNeedBreadCount;                //빵 몇 개 살 지
    public List<GameObject> myBreadList;        //소지한 빵 ( ~3개)
    public GameObject paperBag;
    public bool isToGo;                         //포장인지 홀인지
    public Transform displayTargetSpot;         //진열대 대기 위치 잡기 위한 트랜스폼

    //UI 텍스트
    public GameObject emoji;
    public GameObject speechBalloon;
    public GameObject cashMachine;
    public GameObject hallTable;
    public GameObject breadImg;
    public Text breadNeedNum;



    public CustomerStatePattern statePattern { get; private set; }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();  //애니메이터 제거
        canvas = transform.Find("Canvas");
        //paperBag = transform.Find("PaperBag").gameObject; //페이퍼백 제거(리소스 제거)

        InitStatePattern();
    }

    private void FixedUpdate()
    {
        statePattern?.FixedUpdateState();
    }

    void Update()
    {
        statePattern?.UpdateState();
        canvas.LookAt(GameManager.instance.uiCamera.transform);   //UI 카메라 방향으로 고정

    }

    void InitStatePattern()
    {
        //상태 등록
        CustomerController customerController = GetComponent<CustomerController>();

        statePattern = new CustomerStatePattern(CUSTOMER_STATE.IDLE_IN_SPAWNER, new CustomerStateIdleSpawner(customerController));
        statePattern.AddState(CUSTOMER_STATE.IDLE_IN_DISPLAY, new CustomerStateIdleDisplay(customerController));
        statePattern.AddState(CUSTOMER_STATE.IDLE_IN_CASHIER, new CustomerStateIdleCashier(customerController));
        statePattern.AddState(CUSTOMER_STATE.MOVE_TO_DISPLAY, new CustomerStateMoveToDisplay(customerController));
        statePattern.AddState(CUSTOMER_STATE.MOVE_TO_CASHIER, new CustomerStateMoveToCashier(customerController));
        statePattern.AddState(CUSTOMER_STATE.MOVE_TO_HALL, new CustomerStateMoveToHall(customerController));
        statePattern.AddState(CUSTOMER_STATE.EAT_IN_HALL, new CustomerStateEatInHall(customerController));
        statePattern.AddState(CUSTOMER_STATE.TOGO, new CustomerStateToGo(customerController));

        statePattern.currentBaseState?.OnEnterState();
    }

}
