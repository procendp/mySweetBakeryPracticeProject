using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum CUSTOMER_STATE
{
    IDLE_IN_SPAWNER,    //������ ���
    IDLE_IN_DISPLAY,    //������ ���
    IDLE_IN_CASHIER,    //���� ���
    MOVE_TO_DISPLAY,    //�� ������� �̵�
    MOVE_TO_CASHIER,    //����� �̵� (1.����, 2.Ȧ�Ļ�)
    MOVE_TO_HALL,       //Ȧ�� �̵�
    EAT_IN_HALL,        //Ȧ���� �Ļ�
    TOGO                //�����ؼ� ����
}

public abstract class CustomerBaseState
{
    protected CustomerController customerController { get; private set; }
    public CustomerBaseState(CustomerController cc)
    {
        this.customerController = cc;
    }

    //���� ����

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
    //public Animator animator;         //�ִϸ����� ����
    private Transform canvas;
    public int myNeedBreadCount;                //�� �� �� �� ��
    public List<GameObject> myBreadList;        //������ �� ( ~3��)
    public GameObject paperBag;
    public bool isToGo;                         //�������� Ȧ����
    public Transform displayTargetSpot;         //������ ��� ��ġ ��� ���� Ʈ������

    //UI �ؽ�Ʈ
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
        //animator = GetComponent<Animator>();  //�ִϸ����� ����
        canvas = transform.Find("Canvas");
        //paperBag = transform.Find("PaperBag").gameObject; //�����۹� ����(���ҽ� ����)

        InitStatePattern();
    }

    private void FixedUpdate()
    {
        statePattern?.FixedUpdateState();
    }

    void Update()
    {
        statePattern?.UpdateState();
        canvas.LookAt(GameManager.instance.uiCamera.transform);   //UI ī�޶� �������� ����

    }

    void InitStatePattern()
    {
        //���� ���
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
