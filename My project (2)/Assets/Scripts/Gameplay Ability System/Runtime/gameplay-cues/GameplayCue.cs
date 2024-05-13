using AbilitySystem;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class GameplayCue
{
    //public float LifeTime { get; set; }
    //public bool IsActivated { get; set; }
    //public float delayTime { get; set; }

    [SerializeField] public float LifeTime = 3.0f;
    [SerializeField] public float delayTime = 0.0f;
    [SerializeField] public bool ShouldFlip = false;

    private bool IsActivated = false;
    public GameObject CuePrefab;
    private GameObject CueInstance;
    //public VisualEffect CueVFXPrefab;
    //private VisualEffect CueVFXInstance;
    public GameObject CueVFXPrefab;
    private GameObject CueVFXInstance;

    private Animator animator;
    private Transform spawnPoint;
    private Transform spawnPoint2;

    private GameObject WeaponObj;
    private Transform castPoint;

    [Tooltip("必須手動啟動且不會在啟動時播放的粒子系統。")]
    public GameObject PSPrefab;
    private GameObject PSInstance;

    public void PrepareCue(AbilitySystemCharacter asc)
    {
        animator = asc.GetComponent<Animator>();
        spawnPoint = asc.GetComponent<CaculateAiming>().GetLeftHandPosition();
        spawnPoint2 = asc.GetComponent<CaculateAiming>().GetRightHandPosition();
        castPoint = asc.GetComponent<CaculateAiming>()._castPoint;
        WeaponObj = asc.GetComponent<CastPointComponent>().SwordColliderPoint;
    }

    public async void ApplyCue(AbilitySystemCharacter asc)
    {
        if (asc.gameObject.tag == "Player")
        {
            animator = asc.GetComponent<Animator>();
            spawnPoint = asc.GetComponent<CaculateAiming>().GetLeftHandPosition();
            spawnPoint2 = asc.GetComponent<CaculateAiming>().GetRightHandPosition();
            castPoint = asc.GetComponent<CaculateAiming>()._castPoint;
            WeaponObj = asc.GetComponent<CastPointComponent>().SwordColliderPoint;
        }

        //if (CuePrefab == null && CueVFXPrefab == null) return;

        IsActivated = true;

        if (delayTime > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        }
        Debug.Log("應用Cue!");
        if (CuePrefab != null)
        {
            CueInstance = GameObject.Instantiate(CuePrefab);
            CueInstance.name = "cueInstance_" + CuePrefab.name;

            CueInstance.transform.SetParent(asc.transform);
            if (spawnPoint != null)
                CueInstance.transform.position = spawnPoint.position;
            else
                CueInstance.transform.position = asc.transform.position;

            if (LifeTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(LifeTime));
                GameObject.Destroy(CueInstance);
            }
        }

        if (CueVFXPrefab != null)
        {
            CueVFXInstance = GameObject.Instantiate(CueVFXPrefab);
            CueVFXInstance.name = "cueInstance_" + CueVFXPrefab.name;


            //CueVFXInstance.transform.SetParent(WeaponObj.transform);
            //CueVFXInstance.transform.position = spawnPoint.position;
            //CueVFXInstance.gameObject.transform.localRotation = WeaponObj.gameObject.transform.localRotation;
            //Debug.Log("LOCALROT: " + CueVFXInstance.gameObject.transform.localRotation);
            //if (ShouldFlip)
            //{
            //    Quaternion RotateAmount = Quaternion.Euler(new Vector3(0, 180, 0));
            //    CueVFXInstance.transform.Rotate(Vector3.up);
            //    Debug.Log("LOCALROT2: " + CueVFXInstance.gameObject.transform.localRotation);
            //}
            //CueVFXInstance.transform.SetParent(asc.transform);

            CueVFXInstance.transform.SetParent(castPoint.transform);
            CueVFXInstance.transform.localPosition = CueVFXPrefab.transform.position;
            CueVFXInstance.transform.localRotation = CueVFXPrefab.transform.rotation;


            //CueVFXInstance.transform.SetParent(asc.transform);
            //CueVFXInstance.transform.position = spawnPoint2.position;

            if (LifeTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(LifeTime));
                GameObject.Destroy(CueVFXInstance.gameObject);
            }
        }

        if (PSPrefab != null)
        {
            PSInstance = GameObject.Instantiate(PSPrefab.gameObject);
            PSInstance.transform.SetParent(asc.transform);
            PSInstance.transform.localPosition = PSPrefab.transform.position;
            PSInstance.transform.localRotation = PSPrefab.transform.rotation;

            var ps = PSInstance.GetComponent<ParticleSystem>();

            if (ps.main.startDelay.constant == 0.0f)
            {
                // wait until next frame because the transform may change
                var m = ps.main;
                var d = ps.main.startDelay;
                d.constant = 0.01f;
                m.startDelay = d;
            }
            ps.Play();

            if (LifeTime > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(LifeTime));
                GameObject.Destroy(PSInstance);
            }
        }
        
    }

    public IEnumerator Update()
    {
        float elapsedTime = 0f; // 計算已經過的時間
        while (IsActivated && elapsedTime < LifeTime)
        {
            //Debug.Log("Ability1 IsActivated");
            CueInstance.transform.position = spawnPoint.position;

            // 累加已經過的時間
            elapsedTime += Time.deltaTime;

            yield return null; // 等待下一幀
        }
        //Debug.Log("Ability1 Is not Activated");
        yield return null;
    }

    public async void RemoveCue(AbilitySystemCharacter asc)
    {
        //Debug.Log("移除Cue!");

        
        //int lifeTime = (int)(LifeTime * 1000);
        //Debug.Log("LifeTime: " + lifeTime);
        //await Task.Delay(lifeTime);

        //IsActivated = false;
        //if (CuePrefab != null)
        //{
        //    GameObject.Destroy(CueInstance);
        //}
        
        //if (CueVFXPrefab != null)
        //{
        //    CueVFXPrefab.Stop();
        //}
        
    }
}


