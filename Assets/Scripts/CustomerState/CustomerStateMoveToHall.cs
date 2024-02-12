using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateMoveToHall : CustomerBaseState
{
    public CustomerController controller;

    public CustomerStateMoveToHall(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()
    {
        //목적지 홀로 정함
        //네비로 무빙
        controller.agent.SetDestination(GameManager.instance.targetEatingInHall.transform.position);

        //애니메이션 바꿈
        //controller.animator.SetTrigger("StackWalk");  //애니메이터 제거

        controller.hallTable.SetActive(false);
        controller.speechBalloon.SetActive(false);
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //목적지 도착 -> 상태 변경
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.EAT_IN_HALL);
        }
    }
    public override void OnExitState()
    {

    }
}
