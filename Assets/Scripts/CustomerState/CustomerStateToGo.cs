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
        //스포너 위치 확인 및 이동
        controller.agent.SetDestination(GameManager.instance.targetSpawnSpot.transform.position);

        //나갈 때 포장 -> 포장백 활성화, 애니메이션 스택 무빙으로 바꿔줌
        if (controller.isToGo)
        {
            //controller.paperBag.SetActive(true);  //페이퍼백 제거(리소스 제거)
            //controller.animator.SetTrigger("StackWalk");  //애니메이터 제거

            //GameManager.instance.audioSource.clip = SoundManager.instance.GetAudioClip(SoundState.CASH);    //사운드 이용 안함(리소스 제거)
            GameManager.instance.audioSource.Play();
        }
        else
        {
            //나갈 때 홀 -> 애니메이션 무빙으로 바꿔줌
            //controller.animator.SetTrigger("DefaultWalk");  //애니메이터 제거
        }

        controller.speechBalloon.SetActive(false);
        controller.cashMachine.SetActive(false);
        controller.emoji.SetActive(true);
        controller.StartCoroutine(InactiveEmoji()); // TOGO하며 1.5초 후 이모지 비활성화

    }
    public override void OnUpdateState()
    {

    }
    public override void OnFixedUpdateState()
    {
        //목적지 도착 -> 상태 변경
        if (controller.agent.velocity.sqrMagnitude >= 0.2f * 0.2f && controller.agent.remainingDistance <= 0.5f)
        {
            controller.statePattern.ChangeState(CUSTOMER_STATE.IDLE_IN_SPAWNER);
        }

    }
    public override void OnExitState()
    {
        //도착하면 비활성화
        controller.transform.gameObject.SetActive(false);
        //controller.paperBag.SetActive(false); //페이퍼백 제거(리소스 제거)
    }

    IEnumerator InactiveEmoji()
    {
        yield return new WaitForSeconds(1.5f);
        controller.emoji.SetActive(false);

    }
}
