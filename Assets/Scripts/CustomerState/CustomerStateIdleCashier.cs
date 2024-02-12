using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerStateIdleCashier : CustomerBaseState
{
    public CustomerController controller;
    private float time = 0f;
    private int customerHaveToPay = 0; //고객이 내야할 금액 / 6
    private bool canCalculateRightAway;  //당장 계산할 수 있는지
    private bool isCameraMoving;

    public CustomerStateIdleCashier(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()     //이동하고 도착해서 이미 멈춘 후, 대기하기 위한 준비 동작
    {
        //StackIdle 애니메이션으로 바꿔줌
        //controller.animator.SetTrigger("StackIdle");    //애니메이터 제거

        if (controller.isToGo)
        {
            if (controller.gameObject == GameManager.instance.waitingQueueForToGo.Peek())        //Peek() : 0번째
            {
                //포장 계산할 놈
                canCalculateRightAway = true;
            }
            else
            {
                //줄 서 있는 놈
                canCalculateRightAway = false;
            }
        }
        else
        {
            controller.hallTable.SetActive(true);
            controller.cashMachine.SetActive(false);

            if (controller.gameObject == GameManager.instance.waitingQueueForEatingHall.Peek())
            {
                //매장 식사 계산할 놈
                canCalculateRightAway = true;

                if (!isCameraMoving)    //1번만 하기 위한 장치
                {
                    isCameraMoving = true;
                    CameraController.instance.enumCameraTarget = CAMERA_TARGET.HALL;
                    controller.StartCoroutine(CameraController.instance.ChangeTargetToReturnToPlayer());
                }
            }
            else
            {
                //줄 서 있는 놈
                canCalculateRightAway = false;
            }
        }

        //고객이 구매할 빵 개수 정보 * 6원(장당)
        customerHaveToPay = customerController.myNeedBreadCount * 6;

        ///홀 식사다 = 포장 UI -> 홀 UI
    }
    public override void OnUpdateState()    //서서 하는 동작
    {
        time += Time.deltaTime;

        //플레이어 온 거 확인
        if (GameManager.instance.isColliderCashier)   //플레이어가 계산대에 왔고
        {
            if (canCalculateRightAway)  // 0번째 놈에 대한 계산
            {
                //계산 시작
                if (controller.isToGo)
                {
                    if (time > 0.5f)    //0.5초마다 
                    {
                        //계산됨 확인

                        //빵 사라지는 로직
                        customerController.myBreadList[customerController.myNeedBreadCount - 1].SetActive(false);
                        customerController.myNeedBreadCount--;

                        //페이퍼백 활성화
                        //GameManager.instance.paperBag.SetActive(true);      //리소스 제거

                        time = 0f;
                    }

                    if (customerController.myNeedBreadCount == 0)    //포장 다 됐으면
                    {
                        //GameManager.instance.paperBag.GetComponent<Animator>().SetTrigger("isClose");      //리소스 제거
                        customerController.StartCoroutine(InactivePaperBag());  //페이퍼백 시간차 두고 사라짐
                    
                        //TOGO로 상태변경
                        controller.statePattern.ChangeState(CUSTOMER_STATE.TOGO);
                    }
                }
            }
        }

        if (canCalculateRightAway && !controller.isToGo)
        {
            //홀 생성 됐는지, //홀 -> 테이블 비어있는지 확인
            if (GameManager.instance.isBuildHall && !GameManager.instance.isThereCustomerInHall)
            {
                //갈 수 있어
                controller.statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_HALL);
            }
        }
    }
    public override void OnFixedUpdateState()
    {
        //커스터머 몸을 캐셔쪽으로 바라보게 함
        Vector3 dir = GameManager.instance.cashDesk.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }
    public override void OnExitState()
    {
        time = 0f;

        if (controller.isToGo)
        {
            if (canCalculateRightAway)
            {
                GameManager.instance.waitingQueueForToGo.Dequeue();
                GameManager.instance.StateRefresh(controller.isToGo);   //여기서 고객 하나가 계산이 끝날 때, 큐마다 한칸씩 전진하며 상태 변화
            }
        }
        else
        {
            if (canCalculateRightAway)
            {
                controller.StartCoroutine(WaitToGoToHall());
            }
        }
    }

    IEnumerator InactivePaperBag()
    {
        yield return new WaitForSeconds(0.5f);
        //GameManager.instance.paperBag.SetActive(false);      //리소스 제거

        //스택 머니 이미지 구축
        for (int i = 0; i < GameManager.instance.moneyCashierList.Count; i++)
        {
            if (GameManager.instance.moneyCashierList[i].activeSelf)    //이미 활성화된 돈은 건너뛰고
            {
                continue;
            }
            else if(customerHaveToPay > 0)  //지불할 금액이 남아있다면
            {
                if (GameManager.instance.arrowList[2].activeSelf)
                {
                    GameManager.instance.arrowList[2].SetActive(false);
                    GameManager.instance.arrowList[3].SetActive(true);
                }

                GameManager.instance.moneyCashierList[i].SetActive(true);
                GameManager.instance.cashierStackMoneyCollider.SetActive(true);
                customerHaveToPay--;
            }
        }
    }

    IEnumerator WaitToGoToHall()
    {
        yield return new WaitForSeconds(3.0f);

        GameManager.instance.waitingQueueForEatingHall.Dequeue();
        GameManager.instance.StateRefresh(controller.isToGo);   //여기서 고객 하나가 계산이 끝날 때, 큐마다 한칸씩 전진하며 상태 변화
    }
}
