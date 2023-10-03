using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AbilityObjectPool : MonoBehaviour
{
    public static AbilityObjectPool Instance { get; private set; }

    public ObjectPool<GameObject> objectPool;

    //public Transform _castPoint;

    public List<GameObject> ObjectList;

    public int GetIndex = 0;

    public int ReleaseIndex = 0;

    public Dictionary<int, ObjectPool<GameObject>> _objectPools = new Dictionary<int, ObjectPool<GameObject>>();
    

    private void Awake()
    {
        Instance = this;
    }

    //public void LoadToPool(Dictionary<int, List<GameObject>> abilityObjectDict)
    //{
    //    foreach (var abilityDict in abilityObjectDict)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
    //                () =>
    //                {
    //                    GameObject go = Instantiate(abilityDict.Value[i]);
    //                    go.SetActive(false);

    //                    return go;
    //                },
    //                (go) =>
    //                {
    //                    go.SetActive(true);
    //                },
    //                (go) =>
    //                {
    //                    go.SetActive(false);
    //                },
    //                (go) =>
    //                {
    //                    Destroy(go);
    //                },
    //                true,
    //                _initialPoolSize,
    //                5
    //            );
    //            _objectPools.Add(index++, objectPool);

    //        }
    //    }
    //}

    public void Start()
    {
        
    }

    public void InitPool()
    {
        for (int i = 0; i < ObjectList.Count; i++)
        {
            _objectPools.Add(i, new ObjectPool<GameObject>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, true, 5, 10));
        }
        Debug.Log($"物件池字典目前數量 {_objectPools.Count}");
    }
    /// <summary>
    /// 先指派 assignIndex 來指定要選取哪一個pool
    /// </summary>
    /// <returns></returns>
    private GameObject CreatePooledObject()
    {
        GameObject go = Instantiate(ObjectList[GetIndex]);

        go.SetActive(false);
        return go;
    }

    private void OnTakeFromPool(GameObject go)
    {
        go.SetActive(true);

    }
    private void OnReturnToPool(GameObject go)
    {
        go.SetActive(false);

    }
    private void OnDestroyObject(GameObject go)
    {
        //Destroy(go);
    }
    //public void LoadToPool(GameObject abilityObject)
    //{
    //    objectPool = new ObjectPool<GameObject>(
    //        () =>
    //        { // 創建物件時執行的動作 
    //            GameObject go = Instantiate(ObjectList[index]);
    //            go.SetActive(false);

    //            return go;
    //        },
    //        (go) =>
    //        { // 拿取物件時執行的動作
    //            go.SetActive(true);
    //        },
    //        (go) =>
    //        { // 回收物件時執行的動作
    //            go.SetActive(false);
    //        },
    //        (go) =>
    //        { // 刪除物件時執行的動作
    //            Destroy(go);
    //        },
    //        true,
    //        5,
    //        10
    //    );
    //    _objectPools.Add(index++, objectPool);
    //}

    //public void Check1()
    //{
    //    foreach (var kvp in _objectPools)
    //    {
    //        int poolIndex = kvp.Key;
    //        ObjectPool<GameObject> pool = kvp.Value;
    //        int count = pool.CountInactive;
    //        Debug.Log($"物件池 {poolIndex} 中的物件數：{count}");
    //    }
    //}
    
}
