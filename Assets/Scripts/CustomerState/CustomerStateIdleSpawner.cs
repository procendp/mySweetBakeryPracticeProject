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
        //이걸 받아옴으로써 MonoBehaviour 활용이 가능함
        controller = cc;
    }
    public override void OnEnterState()
    {
        controller.myNeedBreadCount = Random.Range(1, 4);  //필요 빵 개수 랜덤 생성 (1 ~ 3개)     //오브젝트 풀링으로 재사용 시 빵 필요 개수 초기화
        controller.isToGo = Convert.ToBoolean(Random.Range(0, 3));  //포장, 매장 식사 랜덤 지정<총 3개, 그 중 매장식사 확률 : 1/3>   //오브젝트 풀링으로 재사용 시 빵 필요 개수 초기화
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
