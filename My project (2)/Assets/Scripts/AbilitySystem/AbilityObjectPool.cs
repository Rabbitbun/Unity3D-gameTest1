using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AbilityObjectPool : MonoBehaviour
{
    public static AbilityObjectPool Instance { get; private set; }

    [SerializeField] private int _initialPoolSize = 5;

    public List<GameObject> ObjectList;

    public ObjectPool<GameObject> objectPool;

    private int index = 0;

    [SerializeField] public Dictionary<int, ObjectPool<GameObject>> _objectPools = new Dictionary<int, ObjectPool<GameObject>>();
    

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
    public void LoadToPool(GameObject abilityObject)
    {
        objectPool = new ObjectPool<GameObject>(
            () =>
            { // 創建物件時執行的動作 
                GameObject go = Instantiate(ObjectList[index]);
                Debug.Log(go.gameObject.name);
                go.SetActive(false);

                return go;
            },
            (go) =>
            { // 拿取物件時執行的動作
                go.SetActive(true);
            },
            (go) =>
            { // 回收物件時執行的動作
                go.SetActive(false);
            },
            (go) =>
            { // 刪除物件時執行的動作
                Destroy(go);
            },
            true,
            _initialPoolSize,
            6
        );
        _objectPools.Add(index++, objectPool);
    }

    public int Check1()
    {
        return _objectPools[0].CountAll;
    }
    
}
