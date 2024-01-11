using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldown : MonoBehaviour
{
    // 通知冷卻完成的事件
    public event System.Action OnAbilityReady;

    int abilityCapacity = 0;
    public List<float> coolDownDurations;
    public List<float> coolDownTimeLefts;
    // 技能是否在冷卻中
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
    //    AbilityReady();
    //}

    /// <summary>
    /// Initialize Ability cooldown time duration
    /// </summary>
    public void Initialize(List<AbilityObject> objList)
    {
        abilityCapacity = objList.Count;
        coolDownDurations = new List<float>(abilityCapacity);
        coolDownTimeLefts = new List<float>(abilityCapacity);
        IsOnCooldown = new List<bool>(abilityCapacity);

        for (int i = 0; i < objList.Count; i++)
        {
            coolDownDurations.Add(objList[i].Cooldown);
            coolDownTimeLefts.Add(0f);
            IsOnCooldown.Add(false);
        }
    }

    private void Update()
    {
        for (int i = 0; i < abilityCapacity; i++) 
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

    public void AbilityReady()
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
