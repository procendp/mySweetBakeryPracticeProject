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
        //ó����
        //controller.animator.SetTrigger("DefaultIdle");    //�ִϸ����� ����

        //UI ����
        controller.speechBalloon.SetActive(true);
        controller.breadImg.SetActive(true);    //���̹��� + ������
        controller.breadNeedNum.text = controller.myNeedBreadCount.ToString();
        
        breadCount = controller.myNeedBreadCount;
    }
    public override void OnUpdateState()
    {
        int displayBreadCount = 0;  //������ ���� ����
        time += Time.deltaTime;

        if (time > 0.5f)
        {
            //���� �� ������
            for (int i = 0; i < GameManager.instance.displayBreadList.Count; i++)
            {
                if (GameManager.instance.displayBreadList[i].activeSelf)
                {
                    displayBreadCount++;
                }
            }

            if (displayBreadCount > 0 && !isBreadActive)
            {
                //controller.animator.SetTrigger("StackIdle");  //�ִϸ����� ����
                isBreadActive = true;
            }

            //�� ��������

            //�����뿡 �� �ְ�
            if (displayBreadCount > 0)
            {
                controller.myBreadList[count].SetActive(true);  //�� �� Ȱ��ȭ
                count++;
                //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.GET_OBJECT);    //���� �̿� ����(���ҽ� ����)
                GameManager.instance.audioSource.Play();

                GameManager.instance.displayBreadList[displayBreadCount - 1].SetActive(false);  //������ �� ��Ȱ��ȭ

                breadCount--;
                controller.breadNeedNum.text = breadCount.ToString();   //�� �������� ���� ����
            }

            if (count >= controller.myNeedBreadCount)
            {
                controller.statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_CASHIER);
            }

            ///�� �� �� �ִ��� Ȯ��
            ///�� ����
            ///moveToCashier�� ���� ��ȯ

            time = 0;
        }


    }
    public override void OnFixedUpdateState()
    {
        //Ŀ���͸� ���� ������������ �ٶ󺸰� ��
        Vector3 dir = GameManager.instance.basketStore.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);  //�� ����, ���ƺ����ϴ� ����, ȸ�� �ӵ�
    }
    public override void OnExitState()
    {
        isBreadActive = false;
        time = 0;

        GameManager.instance.ChangeDisplaySpotUse(controller.displayTargetSpot, false); //���� ������ ��⼮���� ĳ�ŷ� ����� �� �� �¼�(false)�̶�� �˷���

        //ĳ�ŷ� ��� ���� ����ٿ� ���� ����
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
