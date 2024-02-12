using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerStateEatInHall : CustomerBaseState
{
    public CustomerController controller;
    private float time = 0f;
    private int customerHaveToPay = 0;

    public CustomerStateEatInHall(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()
    {
        //�ڼ����ľɴ� �ִϸ��̼����� �ٲ���
        //controller.animator.SetTrigger("Sitting_Talking");    //�ִϸ����� ����

        //�� ��Ȱ��ȭ�ϰ�
        for (int i = 0; i < controller.myBreadList.Count; i++) 
        {
            controller.myBreadList[i].SetActive(false);
        }

        //���̺� �� �� �ø�
        GameManager.instance.tableInHall.GetChild(0).gameObject.SetActive(true);    //TableBread Ȱ��ȭ

    }
    public override void OnUpdateState()
    {
        time += Time.deltaTime;

        //�����ð����� �԰��� ���� ��
        //���� GOTO�� �ٲ���
        if (time > 5.0f) 
        {
            time = 0f;
            controller.statePattern.ChangeState(CUSTOMER_STATE.TOGO);
        }
    }
    public override void OnFixedUpdateState()
    {
        //�ڼ� ���� �����ɰ�
        Vector3 dir = GameManager.instance.tableInHall.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5);
    }
    public override void OnExitState()
    {
        time = 0;
        customerHaveToPay = controller.myNeedBreadCount * 6;

        ///���̺� �� �� -> ������
        GameManager.instance.tableInHall.GetChild(0).gameObject.SetActive(false);   //TableBread
        GameManager.instance.tableInHall.GetChild(1).gameObject.SetActive(true);    //TableTrash

        if (!GameManager.instance.playerArrow.activeSelf)
        {
            GameManager.instance.arrowList[5].SetActive(true);
            GameManager.instance.playerArrow.SetActive(true);
        }

        ///�� ����
        for (int i = 0; i < GameManager.instance.moneyHallList.Count; i++)
        {
            if (GameManager.instance.moneyHallList[i].activeSelf)
            {
                continue;
            }
            
            if (customerHaveToPay > 0)
            {
                customerHaveToPay--;
                GameManager.instance.moneyHallList[i].SetActive(true);
                GameManager.instance.hallStackMoneyCollider.SetActive(true);
            }
        }
    }
}
