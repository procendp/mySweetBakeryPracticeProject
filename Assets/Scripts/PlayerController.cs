using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public VariableJoystick joystick;
    public float speed = 2.0f;
    public List<GameObject> m_breadList;
    private float timeOven = 0f;
    private float timeGetMoney = 0f;
    private float timePayMoney = 0f;                    //UI �� �پ��� �� Ȯ�� ����
    private int myBreadCount = 0;
    private bool isGetCashierMoney = false;             //ĳ�� ���� �Ա� �����ߴ���
    private bool isGetHallMoney = false;                //Ȧ ���� �Ա� �����ߴ���
    private int activeMoneyCashierListCnt = 0;          //Ȱ��ȭ�� ���� ����
    private int buildHallCost = 30;
    private bool isOpen = false;

    //private AudioSource audioSource;

    Rigidbody rigid;
    //public Animator animator;
    Vector3 moveVector;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        GameManager.instance.myIncome = 35;
        GameManager.instance.txtTotalMoney.text = GameManager.instance.myIncome.ToString();
    }

    private void Update()
    {

        //ĳ�� �Ӵ�
        if (isGetCashierMoney)
        {
            timeGetMoney += Time.deltaTime;

            if (timeGetMoney > 0.1f)
            {
                GameManager.instance.moneyCashierList[activeMoneyCashierListCnt].SetActive(false);
                activeMoneyCashierListCnt--;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.COST_MONEY);        //���� �̿� ����(���ҽ� ����)
                GameManager.instance.audioSource.Play();

                timeGetMoney = 0f;
            }

            if (activeMoneyCashierListCnt < 0)
            {
                isGetCashierMoney = false;
                GameManager.instance.cashierStackMoneyCollider.gameObject.SetActive(false);
            }
        }

        //Ȧ �Ӵ�
        if (isGetHallMoney)
        {
            timeGetMoney += Time.deltaTime;

            if (timeGetMoney > 0.1f)
            {
                GameManager.instance.moneyHallList[activeMoneyCashierListCnt].SetActive(false);
                activeMoneyCashierListCnt--;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.COST_MONEY);    //���� �̿� ����(���ҽ� ����)
                GameManager.instance.audioSource.Play();

                timeGetMoney = 0f;
            }

            if (activeMoneyCashierListCnt < 0)
            {
                isGetHallMoney = false;
                GameManager.instance.hallStackMoneyCollider.gameObject.SetActive(false);
            }
        }

        //Ȧ ������ٸ�
        if (GameManager.instance.isBuildHall)
        {
            //30�� ���� ��� ON
            timePayMoney += Time.deltaTime;

            if (timePayMoney > 0.03f)
            {
                if (buildHallCost > 0)
                {
                    //��ü UI
                    buildHallCost--;
                    GameManager.instance.myIncome--;
                    GameManager.instance.txtTotalMoney.text = GameManager.instance.myIncome.ToString();

                    //Ȧ UI
                    GameManager.instance.txtBulidHallCost.text = buildHallCost.ToString();
                }
                else if (buildHallCost <= 0)
                {
                    //Floor_03�� �̹��� ��ȯ
                    GameManager.instance.hall.transform.GetChild(0).gameObject.SetActive(false);    //Floor_02
                    GameManager.instance.hall.transform.GetChild(1).gameObject.SetActive(true);     //Floor_03

                    if (!isOpen)
                    {
                        //�ѹ��� �Ҹ����Բ�
                        //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.SUCCESS);    //���� �̿� ����(���ҽ� ����)
                        GameManager.instance.audioSource.Play();
                        //GameManager.instance.audioSource.PlayOneShot(SoundManager.instance.GetAudioClip(SoundState.TRASH)); //���� �� �� ���� ���     //���� �̿� ����(���ҽ� ����)

                        isOpen = true;
                    }
                }

                timePayMoney = 0f;
            }
        }
    }

    void FixedUpdate()
    {
        float x = -joystick.Horizontal;
        float z = -joystick.Vertical;

        //JoyStick

        moveVector = new Vector3(x, 0, z) * speed * Time.deltaTime;
        rigid.MovePosition(moveVector + rigid.position);

        //Position
        if (moveVector.sqrMagnitude == 0)
        {
            return;
        }

        //Rotation
        Quaternion dirQuaternion = Quaternion.LookRotation(moveVector);
        Quaternion moveQuaternion = Quaternion.Slerp(rigid.rotation, dirQuaternion, 0.3f);
        rigid.MoveRotation(moveQuaternion);
    }

    private void LateUpdate()
    {
        ///�ִϸ����� ����
        //if (myBreadCount == 0 && !animator.GetBool("isDefault"))
        //{
        //    animator.SetBool("isDefault", true);
        //    animator.SetBool("isStack", false);
        //}
        //else if (myBreadCount != 0 && !animator.GetBool("isStack"))
        //{
        //    animator.SetBool("isStack", true);
        //    animator.SetBool("isDefault", false);
        //}

        ////animator ������Ʈ
        //if (myBreadCount == 0)
        //{
        //    animator.SetFloat("move", moveVector.sqrMagnitude);
        //}
        //else
        //{
        //    animator.SetFloat("handleBread", moveVector.sqrMagnitude);
        //}
    }

    private void OnCollisionStay(Collision collision)
    {

        //�� ���� ����� �浹�ϰ� �ְ�, 0.5�� �Ѿ ��
        if (collision.gameObject.layer == LayerMask.NameToLayer("BasketWithOven"))
        {
            if (GameManager.instance.arrowList[0].activeSelf)
            {
                GameManager.instance.arrowList[0].SetActive(false);
                GameManager.instance.arrowList[1].SetActive(true);
            }

            timeOven += Time.deltaTime;

            bool isThereBread = false;
            int ovenBreadCnt = 0;

            if (myBreadCount > 8)
            {
                return;
            }

            if (GameManager.instance.storeBreadList.Count == 0)
            {
                return;
            }
            else
            {
                //������ �� �ְ�
                for (int i = 0; i < GameManager.instance.storeBreadList.Count; i++)
                {
                    if (GameManager.instance.storeBreadList[i].activeSelf)
                    {
                        isThereBread = true;
                        ovenBreadCnt = i;
                        break;
                    }
                }
            }

            if (isThereBread && myBreadCount < 8 && timeOven > 0.3f)
            {
                //�÷��̾� �� Get
                m_breadList[myBreadCount].SetActive(true);
                myBreadCount++;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.GET_OBJECT);    //���� �̿� ����(���ҽ� ����)
                GameManager.instance.audioSource.Play();

                GameManager.instance.storeBreadList[ovenBreadCnt].SetActive(false);
                timeOven = 0;
            }

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("BasketStore"))
        {
            if (myBreadCount == 0)
            {
                return;
            }
            else
            {
                //���� �� ������ ����
                timeOven += Time.deltaTime;

                int storeNum = 0;   //������ �� ����
                for (int i = 0; i < GameManager.instance.displayBreadList.Count; i++)   //8
                {
                    if (GameManager.instance.displayBreadList[i].activeSelf)
                    {
                        storeNum++;
                    }
                }

                if (storeNum != 8 && timeOven > 0.5f)
                {
                    GameManager.instance.displayBreadList[storeNum].SetActive(true);
                    m_breadList[myBreadCount - 1].SetActive(false);

                    //�÷��̾� ���� �������
                    myBreadCount--;

                    //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.PUT_OBJECT);    //���� �̿� ����(���ҽ� ����)
                    GameManager.instance.audioSource.Play();

                    timeOven = 0;

                    if (myBreadCount == 0)
                    {
                        if (GameManager.instance.arrowList[1].activeSelf)
                        {
                            GameManager.instance.arrowList[1].SetActive(false);
                            GameManager.instance.arrowList[2].SetActive(true);
                        }
                    }
                }
            }
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("CashDesk"))
        {
            GameManager.instance.isColliderCashier = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //�浹 ����� �ð� �ʱ�ȭ
        timeOven = 0;
        GameManager.instance.isColliderCashier = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        int myMoney = 0;

        if (other.gameObject.layer == LayerMask.NameToLayer("GetMoneyColliderCashier"))
        {
            if (GameManager.instance.arrowList[3].activeSelf)
            {
                GameManager.instance.arrowList[3].SetActive(false);
                GameManager.instance.arrowList[4].SetActive(true);
            }

            for (int i = 0; i < GameManager.instance.moneyCashierList.Count; i++)
            {
                if (GameManager.instance.moneyCashierList[i].activeSelf)
                {
                    myMoney++;
                }
            }

            isGetCashierMoney = true;

            activeMoneyCashierListCnt = myMoney - 1;
            GameManager.instance.myIncome += myMoney;
            GameManager.instance.txtTotalMoney.text = GameManager.instance.myIncome.ToString();

        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("GetMoneyColliderHall"))
        {
            for (int i = 0; i < GameManager.instance.moneyHallList.Count; i++)
            {
                if (GameManager.instance.moneyHallList[i].activeSelf)
                {
                    myMoney++;
                }
            }

            isGetHallMoney = true;

            activeMoneyCashierListCnt = myMoney - 1;
            GameManager.instance.myIncome += myMoney;
            GameManager.instance.txtTotalMoney.text = GameManager.instance.myIncome.ToString();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Table"))
        {
            if (GameManager.instance.arrowList[5].activeSelf)
            {
                GameManager.instance.arrowList[5].SetActive(false);
                GameManager.instance.arrowList[6].SetActive(true);
                StartCoroutine(CameraController.instance.ChangeTargetToReturnToPlayer());
                GameManager.instance.tableInHall.GetChild(1).gameObject.SetActive(false);   //Trash false
                GameManager.instance.oneHundredSpot.gameObject.SetActive(true);
                CameraController.instance.enumCameraTarget = CAMERA_TARGET.ONEHUNDRED_MONEYSPOT;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.TRASH);    //���� �̿� ����(���ҽ� ����)
                GameManager.instance.audioSource.Play();
            }
        }

        if (other.gameObject.name == "Floor_02" && GameManager.instance.myIncome >= buildHallCost)
        {
            if (GameManager.instance.arrowList[4].activeSelf)
            {
                GameManager.instance.arrowList[4].SetActive(false);
                GameManager.instance.playerArrow.SetActive(false);
            }

            GameManager.instance.isBuildHall = true;
            //�� �� ��
            // - �� UI �ؽ�Ʈ ����
            // - Floor_03 Ȱ��ȭ(�̹��� ��ȯ)
        }
    }
}
