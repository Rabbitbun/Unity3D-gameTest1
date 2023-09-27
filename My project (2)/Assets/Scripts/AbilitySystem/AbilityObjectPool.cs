using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AbilityObjectPool : MonoBehaviour
{
    public static AbilityObjectPool Instance { get; private set; }

    [SerializeField] private int _initialPoolSize = 5;

    private int index = 0;

    [SerializeField] public Dictionary<int, ObjectPool<GameObject>> _objectPools = new Dictionary<int, ObjectPool<GameObject>>();


    private void Awake()
    {
        Instance = this;
    }

    public void LoadToPool(Dictionary<int, List<GameObject>> abilityObjectDict)
    {
        foreach (var abilityDict in abilityObjectDict)
        {
            for (int i = 0; i < 4; i++)
            {
                ObjectPool<GameObject> objectPool = new ObjectPool<GameObject>(
                    () =>
                    {
                        GameObject go = Instantiate(abilityDict.Value[i]);
                        go.SetActive(false);

                        return go;
                    },
                    (go) =>
                    {
                        go.SetActive(true);
                    },
                    (go) =>
                    {
                        go.SetActive(false);
                    },
                    (go) =>
                    {
                        Destroy(go);
                    },
                    true, 
                    _initialPoolSize, 
                    5
                );
                _objectPools.Add(index++, objectPool);
                
            }
        }
    }
}
