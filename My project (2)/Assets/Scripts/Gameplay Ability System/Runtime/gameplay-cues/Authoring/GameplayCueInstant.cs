using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayCueInstant : GameplayCueBase<GameplayCueInstantSpec>
{
    public virtual void ApplyFrom(GameplayEffectSpec gameplayEffectSpec)
    {
        if (Triggerable(gameplayEffectSpec.Source))
        {
            var instantCue = CreateSpec(new GameplayCueParameters
            { sourceGameplayEffectSpec = gameplayEffectSpec });
            instantCue?.Trigger();
        }
    }

    public virtual void ApplyFrom(AbstractAbilitySpec abilitySpec, params object[] customArguments)
    {
        if (Triggerable(abilitySpec.Owner))
        {
            var instantCue = CreateSpec(new GameplayCueParameters
            { sourceAbilitySpec = abilitySpec, customArguments = customArguments });
            instantCue?.Trigger();
        }
    }
}

public abstract class GameplayCueInstantSpec : GameplayCueSpec
{
    public GameplayCueInstantSpec(GameplayCueInstant cue, GameplayCueParameters parameters) : base(cue,
        parameters)
    {
    }

    public abstract void Trigger();
}

public abstract class GameplayCueInstantSpec<T> : GameplayCueInstantSpec where T : GameplayCueInstant
{
    public readonly T cue;

    public GameplayCueInstantSpec(T cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
        this.cue = cue;
    }
}
