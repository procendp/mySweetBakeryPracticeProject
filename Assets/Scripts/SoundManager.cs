using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundState
{
    CASH,           //결제 후 TOGO
    COST_MONEY,     //홀 오픈 할 때 지불
    GET_OBJECT,     //오븐에서 빵 Get
    PUT_OBJECT,     //진열대로 빵 Put
    SUCCESS,        //홀 오픈
    TRASH           //홀 쓰레기 치울 때

}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    //private List<AudioClip> soundList = new List<AudioClip>();      //사운드 이용 안함(리소스 제거)

    private Dictionary<SoundState, AudioClip> soundDic = new Dictionary<SoundState, AudioClip>();

    private void Awake()
    {
        instance = this;
        //AddSoundDic();

    }

    void AddSoundDic()
    {
        //for (int i = 0; i < soundList.Count; i++)
        //{
        //    soundDic.Add((SoundState)i, soundList[i]);
        //}
    }

    public AudioClip GetAudioClip(SoundState soundState)
    {
        return soundDic[soundState];
    }
}
