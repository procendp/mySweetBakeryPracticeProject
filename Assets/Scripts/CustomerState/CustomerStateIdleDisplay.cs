using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CustomerStateIdleDisplay : CustomerBaseState
{
    public CustomerController controller;
    public bool isBreadActive = false;
    private float time = 0f;
    private int count = 0;
    private int breadCount = 0;

    public CustomerStateIdleDisplay(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()
    {
        count = 0;
        //처음엔
        //controller.animator.SetTrigger("DefaultIdle");    //애니메이터 제거

        //UI 변경
        controller.speechBalloon.SetActive(true);
        controller.breadImg.SetActive(true);    //빵이미지 + 빵개수
        controller.breadNeedNum.text = controller.myNeedBreadCount.ToString();
        
        breadCount = controller.myNeedBreadCount;
    }
    public override void OnUpdateState()
    {
        int displayBreadCount = 0;  //진열된 빵의 개수
        time += Time.deltaTime;

        if (time > 0.5f)
        {
            //담을 빵 있으면
            for (int i = 0; i < GameManager.instance.displayBreadList.Count; i++)
            {
                if (GameManager.instance.displayBreadList[i].activeSelf)
                {
                    displayBreadCount++;
                }
            }

            if (displayBreadCount > 0 && !isBreadActive)
            {
                //controller.animator.SetTrigger("StackIdle");  //애니메이터 제거
                isBreadActive = true;
            }

            //빵 가져가기

            //진열대에 빵 있고
            if (displayBreadCount > 0)
            {
                controller.myBreadList[count].SetActive(true);  //고객 빵 활성화
                count++;
                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.GET_OBJECT);    //사운드 이용 안함(리소스 제거)
                GameManager.instance.audioSource.Play();

                GameManager.instance.displayBreadList[displayBreadCount - 1].SetActive(false);  //진열대 빵 비활성화

                breadCount--;
                controller.breadNeedNum.text = breadCount.ToString();   //빵 얻을수록 개수 빠짐
            }

            if (count >= controller.myNeedBreadCount)
            {
                controller.statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_CASHIER);
            }

            ///빵 몇 개 있는지 확인
            ///빵 스택
            ///moveToCashier로 상태 변환

            time = 0;
        }


    }
    public override void OnFixedUpdateState()
    {
        //커스터머 몸을 진열대쪽으로 바라보게 함
        Vector3 dir = GameManager.instance.basketStore.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);  //내 방향, 돌아봐야하는 방향, 회전 속도
    }
    public override void OnExitState()
    {
        isBreadActive = false;
        time = 0;

        GameManager.instance.ChangeDisplaySpotUse(controller.displayTargetSpot, false); //고객이 진열대 대기석에서 캐셔로 출발할 때 빈 좌석(false)이라고 알려줌

        //캐셔로 출발 전에 대기줄에 나를 넣음
        if (controller.isToGo)
        {
            GameManager.instance.waitingQueueForToGo.Enqueue(controller.gameObject);
        }
        else
        {
            GameManager.instance.waitingQueueForEatingHall.Enqueue(controller.gameObject);
        }

        controller.breadImg.SetActive(false);
        controller.cashMachine.SetActive(true);
    }
}
