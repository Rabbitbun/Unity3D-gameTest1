using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldown : MonoBehaviour
{
    public event System.Action OnAbilityReady;

    // Scriptable Object
    [SerializeField, ReadOnly] public AbilityObject abilityObject;

    [SerializeField] private float _coolDownDuration;
    [SerializeField] public float CoolDownTimeLeft;
    [SerializeField] private float _nextReadyTime;

    public bool IsOnCoolDown;

    private void Awake()
    {
        abilityObject = GetComponent<Spell>().spellObj;
        IsOnCoolDown = false;
    }

    private void Start()
    {
        Initialize(abilityObject);
        AbilityReady();
    }

    /// <summary>
    /// Initialize Ability icon、texture on UI
    /// </summary>
    /// <param name="abilityObject"></param>
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
