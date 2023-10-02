using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldown : MonoBehaviour
{
    // 通知冷卻完成的事件
    public event System.Action OnAbilityReady;

    // Scriptable Object
    [SerializeField, ReadOnly] public AbilityObject abilityObject;

    // 總冷卻時間
    [SerializeField] private float _coolDownDuration;
    // 倒數的冷卻時間
    [SerializeField] public float CoolDownTimeLeft;
    [SerializeField] private float _nextReadyTime;

    public bool IsOnCoolDown;

    /// <summary>
    /// catch the AbilityObject(scriptable obj)
    /// set the cool down flag false
    /// </summary>
    private void Awake()
    {
        abilityObject = GetComponent<Spell>().spellObj;
        IsOnCoolDown = false;
    }

    /// <summary>
    /// initialize AbilityObject
    /// </summary>
    private void Start()
    {
        Initialize(abilityObject);
        AbilityReady();
    }

    /// <summary>
    /// Initialize Ability cooldown time duration
    /// </summary>
    public void Initialize(AbilityObject abilityObject)
    {
        _coolDownDuration = abilityObject.Cooldown;
    }

    public void AbilityReady()
    {
        IsOnCoolDown = false;
    }
    private IEnumerator CooldownCoroutine()
    {
        while (CoolDownTimeLeft > 0f)
        {
            CoolDownTimeLeft -= Time.deltaTime;
            //float roundedCd = Mathf.Round(coolDownTimeLeft);
            //coolDowntextMeshPro.text = roundedCd.ToString();
            //iconMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
            yield return null;
        }
        AbilityReady();
    }
    public void ButtonTriggered()
    {
        _nextReadyTime = _coolDownDuration + Time.time;
        CoolDownTimeLeft = _coolDownDuration;
        //iconMask.enabled = true;
        //coolDowntextMeshPro.enabled = true;
        IsOnCoolDown = true;
        StartCoroutine("CooldownCoroutine");
    }
}
