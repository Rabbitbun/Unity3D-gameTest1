using AbilitySystem;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class GameplayCue
{
    public GameObject CuePrefab;
    public GameObject CueInstance;

    public void ApplyCue(AbilitySystemCharacter asc)
    {
        if (CuePrefab == null)
        {
            return;
        }

        Debug.Log("應用Cue!");
        CueInstance = GameObject.Instantiate(CuePrefab);
        CueInstance.name = "cueInstance_" + CuePrefab.name;

        CueInstance.transform.SetParent(asc.transform);
        CueInstance.transform.position = asc.transform.position;
    }

    public async void RemoveCue(AbilitySystemCharacter asc)
    {
        Debug.Log("移除Cue!");

        await Task.Delay(3_000);

        GameObject.Destroy(CueInstance);
    }
}


