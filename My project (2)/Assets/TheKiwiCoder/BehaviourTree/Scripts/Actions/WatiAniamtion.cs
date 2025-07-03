using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class WatiAniamtion : ActionNode
{
    public string animationName;
    private float animationLength;

    protected override void OnStart() {
        if (context.animator != null)
        {
            AnimationClip clip = GetAnimationClip(animationName);
            if (clip != null)
            {
                animationLength = clip.length;
            }
            else
            {
                Debug.LogError($"Animation clip {animationName} not found in Animator.");
                animationLength = 0f;
            }
        }
        else
        {
            Debug.LogError("Animator not found.");
            animationLength = 0f;
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (animationLength <= 0f)
        {
            return State.Failure;
        }

        animationLength -= Time.deltaTime;

        if (animationLength <= 0f)
        {
            return State.Success;
        }

        return State.Running;
    }

    private AnimationClip GetAnimationClip(string name)
    {
        foreach (AnimationClip clip in context.animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }
        return null;
    }
}
