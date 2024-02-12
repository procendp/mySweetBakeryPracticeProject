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
    public List<GameObject> displayBreadList;               //������ �� ����Ʈ
    public List<GameObject> targetDisplayList;              //������ �� ���� ��ġ 3�� (TargetDisplay1, 2, 3)
    public List<GameObject> moneyCashierList;               //���� �� �� ����
    public List<GameObject> moneyHallList;                  //Ȧ �� ����
    public Dictionary<Transform, bool> dicDisplaySpot;      //������ ��⼮ ��ųʸ�
    public List<GameObject> arrowList;                      //Ʃ�丮�� ȭ��ǥ ����Ʈ

    public Queue<GameObject> waitingQueueForToGo;           //Ŀ���͸ӵ��� ���� �����ϱ� ���� ��� ��
    public Queue<GameObject> waitingQueueForEatingHall;     //Ŀ���͸ӵ��� Ȧ �̿� �����ϱ� ���� ��� ��

    public bool isColliderCashier;                          //�÷��̾ ĳ�ſ� �����ߴ��� Ȯ�� ����
    public Transform basketStore;
    public Transform cashDesk;
    //public GameObject paperBag;           //���ҽ� ����
    public GameObject hall;
    public Text txtTotalMoney;              //��ü ���� UI �ؽ�Ʈ
    public Text txtBulidHallCost;           //Ȧ ���� ��� UI �ؽ�Ʈ
    public int myIncome;                    //����
    public bool isBuildHall = false;        //Ȧ ���������(�浹�ߴ���)
    public bool isThereCustomerInHall;      //Ȧ�� Ŀ���͸� �ִ���
    public Transform tableInHall;
    public GameObject cashierStackMoneyCollider;
    public GameObject hallStackMoneyCollider;
    public GameObject playerArrow;
    public GameObject oneHundredSpot;

    public Transform targetDisplay1;        //BasketStore ������ ����
    public Transform targetDisplay2;        //BasketStore ������ �߾�
    public Transform targetDisplay3;        //BasketStore ������ ����
    public Transform targetCashDeskToGo;    //���� ���� ����
    public Transform targetCashDeskInHall;  //���� ���� �Ļ� ����
    public Transform targetEatingInHall;    //���� �Ļ� ����
    public Transform targetSpawnSpot;       //Customer Spawn ����

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
            ObjectPoolManage(targetDisplayList[i].transform); //���� �ʱ� 3���� ����
        }
    }

    void Update()
    {
        for (int i = 0; i < dicDisplaySpot.Count; i++)
        {
            if (dicDisplaySpot[targetDisplayList[i].transform] == false)    //������ ��⼮ 3�� �� �� ���� �ִٸ�
            {
                ObjectPoolManage(targetDisplayList[i].transform);   //�ű�� ���� -> ���� ����ų� ����Ǵ� ���� ��ġ ����
            }
        }
    }

    public void ObjectPoolManage(Transform transform)   //������ ��ġ
    {
        //������ ���� ������ ������ ��ġ�� �������ٰ� �νĽ�Ŵ
        ChangeDisplaySpotUse(transform, true);

        //������Ʈ Ǯ ����

        //���� ����
        //ó���� ���Ƿ� 3���� ����
        //ĳ�� ���� ���鼭 �浹�ϸ� �� ���� �浹Ƚ����ŭ ����, ����

        GameObject go = ObjectPoolManager.instance.GetObject();

        // - ���� ��ġ
        go.transform.position = targetSpawnSpot.position;

        //����ִ� ��ġ�� ����

         go.GetComponent<CustomerController>().displayTargetSpot = transform;

        if (go.GetComponent<CustomerController>().statePattern != null)
        {
            go.GetComponent<CustomerController>().statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_SPAWNER);
        }
    }

    public void StateRefresh(bool isToGo)  //0��° �� ��� �� TOGO �ϸ�, ��� ���� ���� MOVE_TO_CASHIER�� ����
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

    public void ChangeDisplaySpotUse(Transform waitingSpot, bool isEmpty)   //������ �� ��⼮ ���� ����
    {
        dicDisplaySpot[waitingSpot] = isEmpty;
    }
}
