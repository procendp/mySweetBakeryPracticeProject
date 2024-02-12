using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerStateMoveToDisplay : CustomerBaseState
{
    public CustomerController controller;

    public CustomerStateMoveToDisplay(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()
    {
        //진열대 목적지 설정 및 이동
        if (controller.gameObject.activeSelf)
        {
            controller.agent.SetDestination(controller.displayTargetSpot.position);
        }

        //애니메이션 바꿔줘야함
        //controller.animator.SetTrigger("DefaultWalk");    //애니메이터 제거
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //목적지 도착하면 상태 바꿔주자
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_DISPLAY);
        }

        ////목적지 3개 확인
        //없는 곳으로 위치하게
    }
    public override void OnExitState()
    {

    }
}
