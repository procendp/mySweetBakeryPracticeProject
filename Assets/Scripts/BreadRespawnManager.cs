using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadRespawnManager : MonoBehaviour
{
    public Transform breadSpawnPoint;
    public GameObject breadPrefab;
    private float time = 0f;
    private Rigidbody rbClone;

    void Update()
    {
        time += Time.deltaTime;

        //1�ʸ��� �� ����
        if (time > 1.0f && GameManager.instance.storeBreadList.Count < 8)
        {
            var breadObj = Instantiate(breadPrefab);    //������ ��

            StartCoroutine(showBreadCoroutine(breadObj));

            breadObj.transform.position = breadSpawnPoint.position;

            rbClone = breadObj.GetComponent<Rigidbody>();
            rbClone.AddForce(transform.forward * 5.5f, ForceMode.Impulse);

            time = 0;
        }
    }

    IEnumerator showBreadCoroutine(GameObject go)
    {
        //�� ������ Ȯ�� ����
        yield return new WaitForSeconds(1f);
        GameManager.instance.storeBreadList.Add(go);
    }
}
