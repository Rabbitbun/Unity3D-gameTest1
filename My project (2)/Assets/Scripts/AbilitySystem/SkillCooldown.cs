using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCooldown : MonoBehaviour
{
    // 通知冷卻完成的事件
    public event System.Action OnSkillReady;

    int SkillCapacity = 0;
    public List<float> coolDownDurations;
    public List<float> coolDownTimeLefts;
    public List<bool> IsOnCooldown;


    //// 總冷卻時間
    //[SerializeField] private float _coolDownDuration;
    //// 倒數的冷卻時間
    //[SerializeField] public float CoolDownTimeLeft;
    //[SerializeField] private float _nextReadyTime;

    /// <summary>
    /// initialize AbilityObject
    /// </summary>
    //private void Start()
    //{
    //    SkillReady();
    //}

    /// <summary>
    /// Initialize Ability cooldown time duration
    /// </summary>
    public void Initialize(List<SkillObject> objList)
    {
        SkillCapacity = objList.Count;
        coolDownDurations = new List<float>(SkillCapacity);
        coolDownTimeLefts = new List<float>(SkillCapacity);
        IsOnCooldown = new List<bool>(SkillCapacity);

        for (int i = 0; i < objList.Count; i++)
        {
            coolDownDurations.Add(objList[i].Cooldown);
            coolDownTimeLefts.Add(0f);
            IsOnCooldown.Add(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < SkillCapacity; i++) 
        {
            coolDownTimeLefts[i] = Mathf.Max(coolDownTimeLefts[i] - Time.deltaTime, 0f);
            if (coolDownTimeLefts[i] > 0f)
            {
                IsOnCooldown[i] = true;
            }
            else
            {
                IsOnCooldown[i] = false;
            }
        }
    }

    public void SkillReady()
    {
        
    }
    //private IEnumerator CooldownCoroutine()
    //{
    //    while (CoolDownTimeLeft > 0f)
    //    {
    //        CoolDownTimeLeft -= Time.deltaTime;
    //        //float roundedCd = Mathf.Round(coolDownTimeLeft);
    //        //coolDowntextMeshPro.text = roundedCd.ToString();
    //        //iconMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    //        yield return null;
    //    }
    //    AbilityReady();
    //}
    //public void ButtonTriggered()
    //{
    //    _nextReadyTime = _coolDownDuration + Time.time;
    //    CoolDownTimeLeft = _coolDownDuration;
    //    //iconMask.enabled = true;
    //    //coolDowntextMeshPro.enabled = true;
    //    //IsOnCoolDown = true;
    //    Debug.Log("Active? " + gameObject.activeInHierarchy);
    //    StartCoroutine("CooldownCoroutine");
    //}
}
