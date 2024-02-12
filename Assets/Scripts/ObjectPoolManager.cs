using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager instance;

    [SerializeField]
    private GameObject customerPref;

    private List<GameObject> customerPrefabList;

    private void Awake()
    {
        instance = this;
        customerPrefabList = new List<GameObject>();
    }

    public GameObject GetObject()
    {
        GameObject obj = null;

        foreach (GameObject prefab in customerPrefabList)
        {
            if (!prefab.activeSelf)
            {
                obj = prefab;
                obj.SetActive(true);

                break;  //�ϳ��� �����ϰ� ����
            }
        }

        if (obj == null) //��� ������ �������̶�� ���� �ϳ� ������
        {
            obj = Instantiate(customerPref, transform);     //transform : ������Ʈ Ǯ�� transform(�θ� transform)   //��� �ڽ����� �¾�Բ�
            customerPrefabList.Add(obj);
        }

        return obj;
    }

}
