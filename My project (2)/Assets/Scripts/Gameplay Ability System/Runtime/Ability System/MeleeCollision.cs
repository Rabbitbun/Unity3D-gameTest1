using AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;

/// <summary>
///  目前沒用到 失效
/// </summary>
public class MeleeCollision : MonoBehaviour
{
    public AbilitySystemCharacter target;

    public AbilitySystemCharacter Source;

    public MeleeAbilityScriptableObject.MeleeAbilitySpec MeleeAbilitySpec;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        target = other.GetComponent<AbilitySystemCharacter>();
        if (target != null)
        {
            //MeleeAbilitySpec.CatchTarget(target);
            
        }
    }
}
