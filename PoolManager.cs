using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class PoolManager
{
    private static Dictionary<GameObject, List<Transform>> pool;
    private static bool poolInitiated = false;
    private static Transform poolObjectHolderTransform;

    static void Init()
    {
        InitiatePool();
    }

    private static void InitiatePool()
    {
        pool = new Dictionary<GameObject, List<Transform>>();
        GameObject poolObjectHolder = new GameObject("PoolGameObjectHolder");
        GameObject.DontDestroyOnLoad(poolObjectHolder);
        poolObjectHolderTransform = poolObjectHolder.transform;
        poolInitiated = true;
    }

    public static Transform Instantiate(GameObject objectToInstantiate)
    {
        if(poolInitiated == false)
        {
            InitiatePool();
        }

        List<Transform> objectList;
        if (pool.TryGetValue(objectToInstantiate, out objectList) == false)
        {
            List<Transform> newList = new List<Transform>();
            GameObject newGO = GameObject.Instantiate(objectToInstantiate);
            Transform newGOTransform = newGO.transform;
            newGOTransform.SetParent(poolObjectHolderTransform);
            GameObject.DontDestroyOnLoad(newGO);
            newList.Add(newGOTransform);

            pool.Add(objectToInstantiate, newList);

            return newGOTransform;
        }

        if(objectList != null)
        {
            int poolItemCount = objectList.Count;
            for (int i = 0; i < poolItemCount; i++)
            {
                if (objectList[i].parent == poolObjectHolderTransform && objectList[i].gameObject.activeSelf == false)
                {
                    objectList[i].gameObject.SetActive(true);
                    return objectList[i];
                }
            }

            GameObject newGO = GameObject.Instantiate(objectToInstantiate);
            Transform newGOTransform = newGO.transform;
            newGOTransform.SetParent(poolObjectHolderTransform);
            GameObject.DontDestroyOnLoad(newGO);
            objectList.Add(newGOTransform);

            return newGOTransform;
        }
        else
        {
            Debug.Log("Failed to create pool list");
        }

        Debug.Log("PoolManager failed to Instantiate");
        return null;
    }

    public static void Destroy(GameObject destroyObject)
    {
        if (poolInitiated == false)
        {
            InitiatePool();
            Debug.Log("PoolManager not initiated yet");
            return;
        }

        Transform destroyObjectTransform = destroyObject.transform;
        destroyObjectTransform.SetParent(poolObjectHolderTransform);
        destroyObject.SetActive(false);
    }
}