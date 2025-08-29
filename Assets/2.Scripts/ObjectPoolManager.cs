using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPoolManager : MonoBehaviour
{
    public GameObject[] itemPrefabs;
    public GameObject shopItemPrefab;
    
    List<GameObject>[] pools;

    public void Init()
    {
        pools = new List<GameObject>[itemPrefabs.Length];

        for (int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(ItemType type)
    {
        int typeIdx = (int)type;
        GameObject obj = null;

        foreach (GameObject item in pools[typeIdx])
        {
            if (!item.activeSelf)
            {
                obj = item;
                item.SetActive(true);
                break;
            }
        }

        if (!obj)
        {
            GameObject newObj = Instantiate(itemPrefabs[typeIdx], transform);
            pools[typeIdx].Add(newObj);
            obj = newObj;
        }

        return obj;
    }
}
