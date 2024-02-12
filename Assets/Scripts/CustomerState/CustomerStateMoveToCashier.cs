using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateMoveToCashier : CustomerBaseState
{
    public CustomerController controller;

    public CustomerStateMoveToCashier(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()     //이동 전
    {
        //이동 전 목표지점 설정 및 이동(계산대 -> 포장 or 홀식사)        
        ///ㅇ복제된 커스터머들이 본인 목표지점 정보를 알고 있어야함 -> CustomerController에서 파악하자
        ///들어갈 자리 비었는지도 확인
        ///비어있지 않다면 줄서기..

        if (controller.isToGo)
        {
            ////포장(TOGO)
            controller.agent.SetDestination(GameManager.instance.targetCashDeskToGo.transform.position);
        }
        else
        {
            ////홀 식사
            controller.agent.SetDestination(GameManager.instance.targetCashDeskInHall.transform.position);
        }

        //애니메이션 스택 무빙으로 바꿔줘, 명령 한번
        //controller.animator.SetTrigger("StackWalk");  //애니메이터 제거

        ///UI를 계산대 모양으로 바꿔줌
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()     //이동 중
    {

        if (controller.isToGo)  //포장이면
        {
            for (int i = 0; i < GameManager.instance.waitingQueueForToGo.Count; i++)
            {
                if (controller.gameObject == GameManager.instance.waitingQueueForToGo.ToArray()[i])    //커스터머가 몇번째 큐에 있는지 파악함
                {
                    //대기줄 뒤로 미루기
                    Vector3 destination = GameManager.instance.targetCashDeskToGo.transform.position + new Vector3(0, 0, i * -1.2f);
                    controller.agent.SetDestination(destination);
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.instance.waitingQueueForEatingHall.Count; i++)
            {
                if (controller.gameObject == GameManager.instance.waitingQueueForEatingHall.ToArray()[i])    //커스터머가 몇번째 큐에 있는지 파악함
                {
                    //대기줄 뒤로 미루기
                    Vector3 destination = GameManager.instance.targetCashDeskInHall.transform.position + new Vector3(0, 0, i * -1.2f);
                    controller.agent.SetDestination(destination);
                }
            }
        }

        //목적지 도착 -> 상태 변경
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_CASHIER);
        }
    }
    public override void OnExitState()
    {

    }
}
