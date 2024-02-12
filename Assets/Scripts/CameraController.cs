using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CAMERA_TARGET
{
    NONE,
    PLAYER,
    HALL,
    ONEHUNDRED_MONEYSPOT,
    RETURN_TO_PLAYER
}
public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public GameObject player;
    public CAMERA_TARGET enumCameraTarget;

    public Transform Hall;
    public Transform moneySpot; //100

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        enumCameraTarget = CAMERA_TARGET.PLAYER;
        //Hall.position = Hall.position + new Vector3(0, 10.0f, 10.0f);
        //MoneySpot.position = MoneySpot.position + new Vector3(0, 10.0f, 10.0f);
    }

    void Update()
    {

        switch (enumCameraTarget)
        {
            case CAMERA_TARGET.NONE:
                Vector3 vector3 = player.transform.position + new Vector3(0, 10.0f, 10.0f);
                transform.position = Vector3.Lerp(transform.position, vector3, Time.deltaTime * 5.0f);
                break;

            case CAMERA_TARGET.PLAYER:
                transform.position = player.transform.position + new Vector3(0, 10.0f, 10.0f);
                break;

            case CAMERA_TARGET.HALL:
                Vector3 vector4 = Hall.transform.position + new Vector3(0, 10.0f, 10.0f);
                transform.position = Vector3.Lerp(transform.position, vector4, Time.deltaTime * 5.0f);
                break;

            case CAMERA_TARGET.ONEHUNDRED_MONEYSPOT:
                Vector3 vector5 = moneySpot.transform.position + new Vector3(0, 10.0f, 10.0f);
                transform.position = Vector3.Lerp(transform.position, vector5, Time.deltaTime * 5.0f);
                break;

            case CAMERA_TARGET.RETURN_TO_PLAYER:
                enumCameraTarget = CAMERA_TARGET.NONE;
                StartCoroutine(ChangeTargetToPlayer());
                break;
        }

        //커스터머가 홀 큐에 들어왔을 때
    }

    IEnumerator ChangeTargetToPlayer()
    {
        yield return new WaitForSeconds(3.0f);
        enumCameraTarget = CAMERA_TARGET.PLAYER;
    }

    public IEnumerator ChangeTargetToReturnToPlayer()
    {
        yield return new WaitForSeconds(3.0f);
        enumCameraTarget = CAMERA_TARGET.RETURN_TO_PLAYER;
    }
}
