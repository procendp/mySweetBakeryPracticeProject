using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateToGo : CustomerBaseState
{
    public CustomerController controller;

    public CustomerStateToGo(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()
    {
        //������ ��ġ Ȯ�� �� �̵�
        controller.agent.SetDestination(GameManager.instance.targetSpawnSpot.transform.position);

        //���� �� ���� -> ����� Ȱ��ȭ, �ִϸ��̼� ���� �������� �ٲ���
        if (controller.isToGo)
        {
            //controller.paperBag.SetActive(true);  //�����۹� ����(���ҽ� ����)
            //controller.animator.SetTrigger("StackWalk");  //�ִϸ����� ����

            //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.CASH);    //���� �̿� ����(���ҽ� ����)
            GameManager.instance.audioSource.Play();
        }
        else
        {
            //���� �� Ȧ -> �ִϸ��̼� �������� �ٲ���
            //controller.animator.SetTrigger("DefaultWalk");  //�ִϸ����� ����
        }

        controller.speechBalloon.SetActive(false);
        controller.cashMachine.SetActive(false);
        controller.emoji.SetActive(true);
        controller.StartCoroutine(InactiveEmoji()); // TOGO�ϸ� 1.5�� �� �̸��� ��Ȱ��ȭ

    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //������ ���� -> ���� ����
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_SPAWNER);
        }

    }
    public override void OnExitState()
    {
        //�����ϸ� ��Ȱ��ȭ
        controller.transform.gameObject.SetActive(false);
        //controller.paperBag.SetActive(false); //�����۹� ����(���ҽ� ����)
    }

    IEnumerator InactiveEmoji()
    {
        yield return new WaitForSeconds(1.5f);
        controller.emoji.SetActive(false);

    }
}
