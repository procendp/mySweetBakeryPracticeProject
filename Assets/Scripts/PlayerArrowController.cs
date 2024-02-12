using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrowController : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = player.position;


        //arrow rotation
        for (int i = 0; i < GameManager.instance.arrowList.Count; i++) 
        {
            if (GameManager.instance.arrowList[i].activeSelf)
            {
                Vector3 targetPos = new Vector3(GameManager.instance.arrowList[i].transform.position.x, 0, GameManager.instance.arrowList[i].transform.position.z);
                Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);

                var dir = targetPos - myPos;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
            }
        }
    }
}
