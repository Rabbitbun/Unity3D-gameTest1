using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameplayTag.Authoring;
using AbilitySystem;
using AbilitySystem.Authoring;

public abstract class GameplayCueBase : ScriptableObject
{
    public string Name;
    public string Description = "";
    public GameplayTagScriptableObject tag;

    public virtual bool Triggerable(AbilitySystemCharacter owner)
    {
        if (owner == null) return false;

        //// 持有【所有】RequiredTags才可觸發
        //if (!owner.HasAnyTags(new GameplayTagSet(RequiredTags)))
        //    return false;

        //// 持有【任意】ImmunityTags不可觸發
        //if (owner.HasAnyTags(new GameplayTagSet(ImmunityTags)))
        //    return false;

        return true;
    }

    //public abstract GameplayCueSpec CreateSpec(GameplayCueParameters parameters);
}

public abstract class GameplayCueBase<T> : GameplayCueBase where T : GameplayCueSpec
{
    public abstract T CreateSpec(GameplayCueParameters parameters);
}

public struct GameplayCueParameters
{
    public GameplayEffectSpec sourceGameplayEffectSpec;

    public AbstractAbilitySpec sourceAbilitySpec;

    public object[] customArguments;
    // AggregatedSourceTags
    // AggregatedTargetTags
    // EffectContext
    // Magnitude
}