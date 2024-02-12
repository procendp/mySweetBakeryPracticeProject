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

                break;  //하나만 생성하고 나옴
            }
        }

        if (obj == null) //모두 나가서 재사용중이라면 새로 하나 만들자
        {
            obj = Instantiate(customerPref, transform);     //transform : 오브젝트 풀의 transform(부모 transform)   //어디 자식으로 태어나게끔
            customerPrefabList.Add(obj);
        }

        return obj;
    }

}
