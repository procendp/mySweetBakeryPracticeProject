using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Camera uiCamera;
    public List<GameObject> storeBreadList = new List<GameObject>();
    public List<GameObject> displayBreadList;               //진열대 빵 리스트
    public List<GameObject> targetDisplayList;              //진열대 빵 스택 위치 3곳 (TargetDisplay1, 2, 3)
    public List<GameObject> moneyCashierList;               //계산대 옆 돈 스택
    public List<GameObject> moneyHallList;                  //홀 돈 스택
    public Dictionary<Transform, bool> dicDisplaySpot;      //진열대 대기석 딕셔너리
    public List<GameObject> arrowList;                      //튜토리얼 화살표 리스트

    public Queue<GameObject> waitingQueueForToGo;           //커스터머들이 포장 결제하기 위한 대기 줄
    public Queue<GameObject> waitingQueueForEatingHall;     //커스터머들이 홀 이용 결제하기 위한 대기 줄

    public bool isColliderCashier;                          //플레이어가 캐셔에 도착했는지 확인 여부
    public Transform basketStore;
    public Transform cashDesk;
    //public GameObject paperBag;           //리소스 제거
    public GameObject hall;
    public Text txtTotalMoney;              //전체 수입 UI 텍스트
    public Text txtBulidHallCost;           //홀 빌드 비용 UI 텍스트
    public int myIncome;                    //수입
    public bool isBuildHall = false;        //홀 만들었는지(충돌했는지)
    public bool isThereCustomerInHall;      //홀에 커스터머 있는지
    public Transform tableInHall;
    public GameObject cashierStackMoneyCollider;
    public GameObject hallStackMoneyCollider;
    public GameObject playerArrow;
    public GameObject oneHundredSpot;

    public Transform targetDisplay1;        //BasketStore 진열대 우측
    public Transform targetDisplay2;        //BasketStore 진열대 중앙
    public Transform targetDisplay3;        //BasketStore 진열대 좌측
    public Transform targetCashDeskToGo;    //계산대 포장 지점
    public Transform targetCashDeskInHall;  //계산대 매장 식사 지점
    public Transform targetEatingInHall;    //매장 식사 지점
    public Transform targetSpawnSpot;       //Customer Spawn 지점

    public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        waitingQueueForToGo = new Queue<GameObject>();
        waitingQueueForEatingHall = new Queue<GameObject>();
        dicDisplaySpot = new Dictionary<Transform, bool>();

        for (int i = 0; i < targetDisplayList.Count; i++)
        {
            dicDisplaySpot.Add(targetDisplayList[i].transform, false);
        }

        for (int i = 0; i < 3; i++)
        {
            ObjectPoolManage(targetDisplayList[i].transform); //게임 초기 3마리 생성
        }
    }

    void Update()
    {
        for (int i = 0; i < dicDisplaySpot.Count; i++)
        {
            if (dicDisplaySpot[targetDisplayList[i].transform] == false)    //진열대 대기석 3곳 중 빈 곳이 있다면
            {
                ObjectPoolManage(targetDisplayList[i].transform);   //거기로 가라 -> 새로 생기거나 재사용되는 고객들 위치 지정
            }
        }
    }

    public void ObjectPoolManage(Transform transform)   //진열대 위치
    {
        //생성된 고객이 가야할 진열대 위치가 정해졌다고 인식시킴
        ChangeDisplaySpotUse(transform, true);

        //오브젝트 풀 관리

        //생성 조건
        //처음에 임의로 3마리 생성
        //캐셔 어디든 들어가면서 충돌하면 그 순간 충돌횟수만큼 생성, 투입

        GameObject go = ObjectPoolManager.instance.GetObject();

        // - 생성 위치
        go.transform.position = targetSpawnSpot.position;

        //비어있는 위치로 가라

         go.GetComponent<CustomerController>().displayTargetSpot = transform;

        if (go.GetComponent<CustomerController>().statePattern != null)
        {
            go.GetComponent<CustomerController>().statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_SPAWNER);
        }
    }

    public void StateRefresh(bool isToGo)  //0번째 고객 계산 후 TOGO 하면, 대기 고객들 전부 MOVE_TO_CASHIER로 변경
    {
        if (isToGo) 
        {
            foreach (var item in waitingQueueForToGo)
            {
                item.GetComponent<CustomerController>().statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_CASHIER);
            }
        }
        else
        {
            foreach (var item in waitingQueueForEatingHall)
            {
                item.GetComponent<CustomerController>().statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_CASHIER);
            }
        }
    }

    public void ChangeDisplaySpotUse(Transform waitingSpot, bool isEmpty)   //진열대 빈 대기석 상태 수정
    {
        dicDisplaySpot[waitingSpot] = isEmpty;
    }
}
