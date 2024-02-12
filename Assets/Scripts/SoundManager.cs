using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundState
{
    CASH,           //���� �� TOGO
    COST_MONEY,     //Ȧ ���� �� �� ����
    GET_OBJECT,     //���쿡�� �� Get
    PUT_OBJECT,     //������� �� Put
    SUCCESS,        //Ȧ ����
    TRASH           //Ȧ ������ ġ�� ��

}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    //private List<AudioClip> soundList = new List<AudioClip>();      //���� �̿� ����(���ҽ� ����)

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
