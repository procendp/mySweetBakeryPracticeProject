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
        //������ Ȧ�� ����
        //�׺�� ����
        controller.agent.SetDestination(GameManager.instance.targetEatingInHall.transform.position);

        //�ִϸ��̼� �ٲ�
        //controller.animator.SetTrigger("StackWalk");  //�ִϸ����� ����

        controller.hallTable.SetActive(false);
        controller.speechBalloon.SetActive(false);
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //������ ���� -> ���� ����
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.EAT_IN_HALL);
        }
    }
    public override void OnExitState()
    {

    }
}
