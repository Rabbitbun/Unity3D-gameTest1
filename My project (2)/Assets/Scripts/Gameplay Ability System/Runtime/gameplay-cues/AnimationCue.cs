using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Animation Cue", menuName = "Gameplay/Animation Cue")]
public class AnimationCue : GameplayCueDurational
{
    //[SerializeField]
    //private Animator _animator;
    //public Animator Animator => _animator;

    [Tooltip("動畫狀態名稱")]
    [SerializeField]
    private string _stateName;
    public string StateName => _stateName;

    public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
    {
        var spec = new AnimationCueSpec(this, parameters);
        //spec.animator = this._animator;
        return spec;
    }
}

public class AnimationCueSpec : GameplayCueDurationalSpec<AnimationCue>
{
    public Animator animator;

    public AnimationCueSpec(AnimationCue cue, GameplayCueParameters parameters) : base(cue, parameters)
    {
        animator = Owner.gameObject.GetComponent<Animator>();
        //animator = cue.AnimatorGameObject.GetComponent<Animator>();
    }

    public override void OnAdd()
    {
        animator.SetTrigger(cue.StateName);
    }

    public override void OnRemove()
    {
        animator.ResetTrigger(cue.StateName);
    }

    public override void OnGameplayEffectActivate()
    {
    }

    public override void OnGameplayEffectDeactivate()
    {
    }

    public override void OnTick()
    {
    }
}