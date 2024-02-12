using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerStateIdleCashier : CustomerBaseState
{
    public CustomerController controller;
    private float time = 0f;
    private int customerHaveToPay = 0; //���� ������ �ݾ� / 6
    private bool canCalculateRightAway;  //���� ����� �� �ִ���
    private bool isCameraMoving;

    public CustomerStateIdleCashier(CustomerController cc) : base(cc)
    {
        controller = cc;
    }
    public override void OnEnterState()     //�̵��ϰ� �����ؼ� �̹� ���� ��, ����ϱ� ���� �غ� ����
    {
        //StackIdle �ִϸ��̼����� �ٲ���
        //controller.animator.SetTrigger("StackIdle");    //�ִϸ����� ����

        if (controller.isToGo)
        {
            if (controller.gameObject == GameManager.instance.waitingQueueForToGo.Peek())        //Peek() : 0��°
            {
                //���� ����� ��
                canCalculateRightAway = true;
            }
            else
            {
                //�� �� �ִ� ��
                canCalculateRightAway = false;
            }
        }
        else
        {
            controller.hallTable.SetActive(true);
            controller.cashMachine.SetActive(false);

            if (controller.gameObject == GameManager.instance.waitingQueueForEatingHall.Peek())
            {
                //���� �Ļ� ����� ��
                canCalculateRightAway = true;

                if (!isCameraMoving)    //1���� �ϱ� ���� ��ġ
                {
                    isCameraMoving = true;
                    CameraController.instance.enumCameraTarget = CAMERA_TARGET.HALL;
                    controller.StartCoroutine(CameraController.instance.ChangeTargetToReturnToPlayer());
                }
            }
            else
            {
                //�� �� �ִ� ��
                canCalculateRightAway = false;
            }
        }

        //���� ������ �� ���� ���� * 6��(���)
        customerHaveToPay = customerController.myNeedBreadCount * 6;

        ///Ȧ �Ļ�� = ���� UI -> Ȧ UI
    }
    public override void OnUpdateState()    //���� �ϴ� ����
    {
        time += Time.deltaTime;

        //�÷��̾� �� �� Ȯ��
        if (GameManager.instance.isColliderCashier)   //�÷��̾ ���뿡 �԰�
        {
            if (canCalculateRightAway)  // 0��° �� ���� ���
            {
                //��� ����
                if (controller.isToGo)
                {
                    if (time > 0.5f)    //0.5�ʸ��� 
                    {
                        //���� Ȯ��

                        //�� ������� ����
                        customerController.myBreadList[customerController.myNeedBreadCount - 1].SetActive(false);
                        customerController.myNeedBreadCount--;

                        //�����۹� Ȱ��ȭ
                        //GameManager.instance.paperBag.SetActive(true);      //���ҽ� ����

                        time = 0f;
                    }

                    if (customerController.myNeedBreadCount == 0)    //���� �� ������
                    {
                        //GameManager.instance.paperBag.GetComponent<Animator>().SetTrigger("isClose");      //���ҽ� ����
                        customerController.StartCoroutine(InactivePaperBag());  //�����۹� �ð��� �ΰ� �����
                    
                        //TOGO�� ���º���
                        controller.statePattern.ChangeState(CUSTOMER_STATE.TOGO);
                    }
                }
            }
        }

        if (canCalculateRightAway && !controller.isToGo)
        {
            //Ȧ ���� �ƴ���, //Ȧ -> ���̺� ����ִ��� Ȯ��
            if (GameManager.instance.isBuildHall && !GameManager.instance.isThereCustomerInHall)
            {
                //�� �� �־�
                controller.statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_HALL);
            }
        }
    }
    public override void OnFixedUpdateState()
    {
        //Ŀ���͸� ���� ĳ�������� �ٶ󺸰� ��
        Vector3 dir = GameManager.instance.cashDesk.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }
    public override void OnExitState()
    {
        time = 0f;

        if (controller.isToGo)
        {
            if (canCalculateRightAway)
            {
                GameManager.instance.waitingQueueForToGo.Dequeue();
                GameManager.instance.StateRefresh(controller.isToGo);   //���⼭ �� �ϳ��� ����� ���� ��, ť���� ��ĭ�� �����ϸ� ���� ��ȭ
            }
        }
        else
        {
            if (canCalculateRightAway)
            {
                controller.StartCoroutine(WaitToGoToHall());
            }
        }
    }

    IEnumerator InactivePaperBag()
    {
        yield return new WaitForSeconds(0.5f);
        //GameManager.instance.paperBag.SetActive(false);      //���ҽ� ����

        //���� �Ӵ� �̹��� ����
        for (int i = 0; i < GameManager.instance.moneyCashierList.Count; i++)
        {
            if (GameManager.instance.moneyCashierList[i].activeSelf)    //�̹� Ȱ��ȭ�� ���� �ǳʶٰ�
            {
                continue;
            }
            else if(customerHaveToPay > 0)  //������ �ݾ��� �����ִٸ�
            {
                if (GameManager.instance.arrowList[2].activeSelf)
                {
                    GameManager.instance.arrowList[2].SetActive(false);
                    GameManager.instance.arrowList[3].SetActive(true);
                }

                GameManager.instance.moneyCashierList[i].SetActive(true);
                GameManager.instance.cashierStackMoneyCollider.SetActive(true);
                customerHaveToPay--;
            }
        }
    }

    IEnumerator WaitToGoToHall()
    {
        yield return new WaitForSeconds(3.0f);

        GameManager.instance.waitingQueueForEatingHall.Dequeue();
        GameManager.instance.StateRefresh(controller.isToGo);   //���⼭ �� �ϳ��� ����� ���� ��, ť���� ��ĭ�� �����ϸ� ���� ��ȭ
    }
}
