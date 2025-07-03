using AbilitySystem;
using UnityEngine;
using AbilitySystem.Authoring;
using GameplayTag.Authoring;
using System;

public class AbilityCollision : MonoBehaviour
{
    public AbilitySystemCharacter target;

    public AbilitySystemCharacter Source;

    public Action<AbilitySystemCharacter> OnHit, OnExit;

    private void OnTriggerEnter(Collider other)
    {
        print("OnTriggerEnter 碰撞到" + other.name);

        AbilitySystemCharacter character = other.transform.root.gameObject.GetComponent<AbilitySystemCharacter>();
        
        //AbilitySystemCharacter character = other.gameObject.GetComponent<AbilitySystemCharacter>();

        if (character != null && character != Source)
        {
            target = character;
            Debug.Log($"Invoking OnHit for {character.gameObject.name}");
            OnHit?.Invoke(character);
            //Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<AbilitySystemCharacter>() != null && other.gameObject.GetComponent<AbilitySystemCharacter>() != Source)
        {
            OnExit?.Invoke(other.gameObject.GetComponent<AbilitySystemCharacter>());
        }
    }
}
