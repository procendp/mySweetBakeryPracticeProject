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
        //������ ������ ���� �� �̵�
        if (controller.gameObject.activeSelf)
        {
            controller.agent.SetDestination(controller.displayTargetSpot.position);
        }

        //�ִϸ��̼� �ٲ������
        //controller.animator.SetTrigger("DefaultWalk");    //�ִϸ����� ����
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //������ �����ϸ� ���� �ٲ�����
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_DISPLAY);
        }

        ////������ 3�� Ȯ��
        //���� ������ ��ġ�ϰ�
    }
    public override void OnExitState()
    {

    }
}
