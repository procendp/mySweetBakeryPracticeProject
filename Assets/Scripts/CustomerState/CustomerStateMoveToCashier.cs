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
    public override void OnEnterState()     //�̵� ��
    {
        //�̵� �� ��ǥ���� ���� �� �̵�(���� -> ���� or Ȧ�Ļ�)        
        ///�������� Ŀ���͸ӵ��� ���� ��ǥ���� ������ �˰� �־���� -> CustomerController���� �ľ�����
        ///�� �ڸ� ��������� Ȯ��
        ///������� �ʴٸ� �ټ���..

        if (controller.isToGo)
        {
            ////����(TOGO)
            controller.agent.SetDestination(GameManager.instance.targetCashDeskToGo.transform.position);
        }
        else
        {
            ////Ȧ �Ļ�
            controller.agent.SetDestination(GameManager.instance.targetCashDeskInHall.transform.position);
        }

        //�ִϸ��̼� ���� �������� �ٲ���, ��� �ѹ�
        //controller.animator.SetTrigger("StackWalk");  //�ִϸ����� ����

        ///UI�� ���� ������� �ٲ���
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()     //�̵� ��
    {

        if (controller.isToGo)  //�����̸�
        {
            for (int i = 0; i < GameManager.instance.waitingQueueForToGo.Count; i++)
            {
                if (controller.gameObject == GameManager.instance.waitingQueueForToGo.ToArray()[i])    //Ŀ���͸Ӱ� ���° ť�� �ִ��� �ľ���
                {
                    //����� �ڷ� �̷��
                    Vector3 destination = GameManager.instance.targetCashDeskToGo.transform.position + new Vector3(0, 0, i * -1.2f);
                    controller.agent.SetDestination(destination);
                }
            }
        }
        else
        {
            for (int i = 0; i < GameManager.instance.waitingQueueForEatingHall.Count; i++)
            {
                if (controller.gameObject == GameManager.instance.waitingQueueForEatingHall.ToArray()[i])    //Ŀ���͸Ӱ� ���° ť�� �ִ��� �ľ���
                {
                    //����� �ڷ� �̷��
                    Vector3 destination = GameManager.instance.targetCashDeskInHall.transform.position + new Vector3(0, 0, i * -1.2f);
                    controller.agent.SetDestination(destination);
                }
            }
        }

        //������ ���� -> ���� ����
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_CASHIER);
        }
    }
    public override void OnExitState()
    {

    }
}
