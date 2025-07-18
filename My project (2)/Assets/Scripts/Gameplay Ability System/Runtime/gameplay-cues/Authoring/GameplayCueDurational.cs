using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayCueDurational : GameplayCueBase<GameplayCueDurationalSpec>
{
    public GameplayCueDurationalSpec ApplyFrom(GameplayEffectSpec gameplayEffectSpec)
    {
        if (!Triggerable(gameplayEffectSpec.Source)) return null;
        var durationalCue = CreateSpec(new GameplayCueParameters
        { sourceGameplayEffectSpec = gameplayEffectSpec });
        return durationalCue;
    }

    public GameplayCueDurationalSpec ApplyFrom(AbstractAbilitySpec abilitySpec, params object[] customArguments)
    {
        if (!Triggerable(abilitySpec.Owner)) return null;
        var durationalCue = CreateSpec(new GameplayCueParameters
        { sourceAbilitySpec = abilitySpec, customArguments = customArguments });
        return durationalCue;
    }
}

public abstract class GameplayCueDurationalSpec : GameplayCueSpec
{
    protected GameplayCueDurationalSpec(GameplayCueDurational cue, GameplayCueParameters parameters) :
        base(cue, parameters)
    {
    }

    public abstract void OnAdd();
    public abstract void OnRemove();
    public abstract void OnGameplayEffectActivate();
    public abstract void OnGameplayEffectDeactivate();
    public abstract void OnTick();
}

public abstract class GameplayCueDurationalSpec<T> : GameplayCueDurationalSpec where T : GameplayCueDurational
{
    public readonly T cue;

    protected GameplayCueDurationalSpec(T cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
        this.cue = cue;
    }
}
