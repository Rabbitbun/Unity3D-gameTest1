using AbilitySystem;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class GameplayCue
{
    public float LifeTime = 3.0f;
    public bool IsActivated = false;

    public Transform SpawnPoint;

    public GameObject CuePrefab;
    public GameObject CueInstance;
    
    public VisualEffect CueVFXPrefab;

    [SerializeField] private Animator animator;

    public void PrepareCue(AbilitySystemCharacter asc)
    {
        animator = asc.GetComponent<Animator>();
        SpawnPoint = asc.GetComponent<CaculateAiming>().GetLeftHandPosition();
    }

    public void ApplyCue(AbilitySystemCharacter asc)
    {
        //if (CuePrefab == null) return;
        IsActivated = true;
        Debug.Log("應用Cue!");
        if (CuePrefab != null)
        {
            CueInstance = GameObject.Instantiate(CuePrefab);
            CueInstance.name = "cueInstance_" + CuePrefab.name;

            //CueInstance.transform.SetParent(asc.transform);
            CueInstance.transform.position = SpawnPoint.position;
        }

    }

    public IEnumerator Update()
    {
        float elapsedTime = 0f; // 計算已經過的時間
        while (IsActivated && elapsedTime < LifeTime)
        {
            Debug.Log("Ability1 IsActivated");
            CueInstance.transform.position = SpawnPoint.position;

            // 累加已經過的時間
            elapsedTime += Time.deltaTime;

            yield return null; // 等待下一幀
        }
        Debug.Log("Ability1 Is not Activated");
        yield return null;
    }

    public async void RemoveCue(AbilitySystemCharacter asc)
    {
        Debug.Log("移除Cue!");

        
        //int lifeTime = (int)(LifeTime * 1000);
        //Debug.Log("LifeTime: " + lifeTime);
        //await Task.Delay(lifeTime);

        IsActivated = false;
        if (CuePrefab != null)
        {
            GameObject.Destroy(CueInstance);
        }
        
        if (CueVFXPrefab != null)
        {
            CueVFXPrefab.Stop();
        }
        
    }
}


