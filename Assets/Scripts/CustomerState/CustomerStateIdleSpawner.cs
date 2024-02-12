using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CustomerStateIdleSpawner : CustomerBaseState
{
    public CustomerController controller;

    public CustomerStateIdleSpawner(CustomerController cc) : base(cc)
    {
        //�̰� �޾ƿ����ν� MonoBehaviour Ȱ���� ������
        controller = cc;
    }
    public override void OnEnterState()
    {
        controller.myNeedBreadCount = Random.Range(1, 4);  //�ʿ� �� ���� ���� ���� (1 ~ 3��)     //������Ʈ Ǯ������ ���� �� �� �ʿ� ���� �ʱ�ȭ
        controller.isToGo = Convert.ToBoolean(Random.Range(0, 3));  //����, ���� �Ļ� ���� ����<�� 3��, �� �� ����Ļ� Ȯ�� : 1/3>   //������Ʈ Ǯ������ ���� �� �� �ʿ� ���� �ʱ�ȭ
        controller.statePattern.ChangeState(CUSTOMER_STATE.MOVE_TO_DISPLAY);
    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {

    }
    public override void OnExitState()
    {
        
    }
}
