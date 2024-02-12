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
        //자세고쳐앉는 애니메이션으로 바꿔줌
        //controller.animator.SetTrigger("Sitting_Talking");    //애니메이터 제거

        //빵 비활성화하고
        for (int i = 0; i < controller.myBreadList.Count; i++) 
        {
            controller.myBreadList[i].SetActive(false);
        }

        //테이블 위 빵 올림
        GameManager.instance.tableInHall.GetChild(0).gameObject.SetActive(true);    //TableBread 활성화

    }
    public override void OnUpdateState()
    {
        time += Time.deltaTime;

        //일정시간동안 먹고나면 상태 끝
        //상태 GOTO로 바꿔줌
        if (time > 5.0f) 
        {
            time = 0f;
            controller.statePattern.ChangeState(CUSTOMER_STATE.TOGO);
        }
    }
    public override void OnFixedUpdateState()
    {
        //자세 방향 돌려앉고
        Vector3 dir = GameManager.instance.tableInHall.transform.position - controller.transform.position;
        controller.transform.rotation = Quaternion.Lerp(controller.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5);
    }
    public override void OnExitState()
    {
        time = 0;
        customerHaveToPay = controller.myNeedBreadCount * 6;

        ///테이블 위 빵 -> 쓰레기
        GameManager.instance.tableInHall.GetChild(0).gameObject.SetActive(false);   //TableBread
        GameManager.instance.tableInHall.GetChild(1).gameObject.SetActive(true);    //TableTrash

        if (!GameManager.instance.playerArrow.activeSelf)
        {
            GameManager.instance.arrowList[5].SetActive(true);
            GameManager.instance.playerArrow.SetActive(true);
        }

        ///돈 스택
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
