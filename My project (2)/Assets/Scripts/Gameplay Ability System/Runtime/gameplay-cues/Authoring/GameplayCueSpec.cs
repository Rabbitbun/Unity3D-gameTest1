using AbilitySystem;
using UnityEngine;

public abstract class GameplayCueSpec
{
    protected readonly GameplayCueBase _cue;
    protected readonly GameplayCueParameters _parameters;
    public AbilitySystemCharacter Owner { get; protected set; }

    public virtual bool Triggerable()
    {
        return _cue.Triggerable(Owner);
    }

    public GameplayCueSpec(GameplayCueBase cue, GameplayCueParameters cueParameters)
    {
        _cue = cue;
        _parameters = cueParameters;
        if (_parameters.sourceGameplayEffectSpec != null)
        {
            Owner = _parameters.sourceGameplayEffectSpec.Source;
        }
        else if (_parameters.sourceAbilitySpec != null)
        {
            Owner = _parameters.sourceAbilitySpec.Owner;
        }
    }


}
