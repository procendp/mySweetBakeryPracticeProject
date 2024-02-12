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
    private float timePayMoney = 0f;                    //UI 돈 줄어드는 것 확인 위함
    private int myBreadCount = 0;
    private bool isGetCashierMoney = false;             //캐셔 돈을 먹기 시작했는지
    private bool isGetHallMoney = false;                //홀 돈을 먹기 시작했는지
    private int activeMoneyCashierListCnt = 0;          //활성화된 돈의 개수
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

        //캐셔 머니
        if (isGetCashierMoney)
        {
            timeGetMoney += Time.deltaTime;

            if (timeGetMoney > 0.1f)
            {
                GameManager.instance.moneyCashierList[activeMoneyCashierListCnt].SetActive(false);
                activeMoneyCashierListCnt--;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.COST_MONEY);        //사운드 이용 안함(리소스 제거)
                GameManager.instance.audioSource.Play();

                timeGetMoney = 0f;
            }

            if (activeMoneyCashierListCnt < 0)
            {
                isGetCashierMoney = false;
                GameManager.instance.cashierStackMoneyCollider.gameObject.SetActive(false);
            }
        }

        //홀 머니
        if (isGetHallMoney)
        {
            timeGetMoney += Time.deltaTime;

            if (timeGetMoney > 0.1f)
            {
                GameManager.instance.moneyHallList[activeMoneyCashierListCnt].SetActive(false);
                activeMoneyCashierListCnt--;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.COST_MONEY);    //사운드 이용 안함(리소스 제거)
                GameManager.instance.audioSource.Play();

                timeGetMoney = 0f;
            }

            if (activeMoneyCashierListCnt < 0)
            {
                isGetHallMoney = false;
                GameManager.instance.hallStackMoneyCollider.gameObject.SetActive(false);
            }
        }

        //홀 만들었다면
        if (GameManager.instance.isBuildHall)
        {
            //30원 차감 모션 ON
            timePayMoney += Time.deltaTime;

            if (timePayMoney > 0.03f)
            {
                if (buildHallCost > 0)
                {
                    //전체 UI
                    buildHallCost--;
                    GameManager.instance.myIncome--;
                    GameManager.instance.txtTotalMoney.text = GameManager.instance.myIncome.ToString();

                    //홀 UI
                    GameManager.instance.txtBulidHallCost.text = buildHallCost.ToString();
                }
                else if (buildHallCost <= 0)
                {
                    //Floor_03로 이미지 전환
                    GameManager.instance.hall.transform.GetChild(0).gameObject.SetActive(false);    //Floor_02
                    GameManager.instance.hall.transform.GetChild(1).gameObject.SetActive(true);     //Floor_03

                    if (!isOpen)
                    {
                        //한번만 소리나게끔
                        //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.SUCCESS);    //사운드 이용 안함(리소스 제거)
                        GameManager.instance.audioSource.Play();
                        //GameManager.instance.audioSource.PlayOneShot(SoundManager.instance.GetAudioClip(SoundState.TRASH)); //사운드 한 번 동시 출력     //사운드 이용 안함(리소스 제거)

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
        ///애니메이터 제거
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

        ////animator 업데이트
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

        //빵 생산 저장고에 충돌하고 있고, 0.5초 넘어갈 때
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
                //생성된 빵 있고
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
                //플레이어 빵 Get
                m_breadList[myBreadCount].SetActive(true);
                myBreadCount++;

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.GET_OBJECT);    //사운드 이용 안함(리소스 제거)
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
                //내가 빵 가지고 있음
                timeOven += Time.deltaTime;

                int storeNum = 0;   //진열대 빵 개수
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

                    //플레이어 빵을 진열대로
                    myBreadCount--;

                    //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.PUT_OBJECT);    //사운드 이용 안함(리소스 제거)
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
        //충돌 벗어나면 시간 초기화
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

                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.TRASH);    //사운드 이용 안함(리소스 제거)
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
            //돈 쓸 때
            // - 돈 UI 텍스트 변경
            // - Floor_03 활성화(이미지 전환)
        }
    }
}
