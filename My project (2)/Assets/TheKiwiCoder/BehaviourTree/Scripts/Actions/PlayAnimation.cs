using UnityEngine;
using TheKiwiCoder;

public class PlayAnimation : ActionNode
{
    public string animationName;

    public bool WaitForAnimationEnd = false;

    private AnimatorStateInfo animatorStateInfo;
    [SerializeField]
    private float elapsedTime;
    public float transitionDuration = 0.25f;
    public int animationLayer = 0;

    protected override void OnStart() {
        if (context.animator != null)
        {
            context.animator.Play(animationName);
            elapsedTime = 0f;
        }
        else
        {
            Debug.LogError("Animator not found.");
        }
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.animator == null)
        {
            return State.Failure;
        }
        if (WaitForAnimationEnd)
        {
            elapsedTime += Time.deltaTime;

            animatorStateInfo = context.animator.GetCurrentAnimatorStateInfo(animationLayer);

            if (animatorStateInfo.IsName(animationName))
            {
                if (elapsedTime >= animatorStateInfo.length)
                {
                    return State.Success;
                }
            }
            else
            {
                context.animator.CrossFade(animationName, transitionDuration, animationLayer);
                elapsedTime = 0f;
                return State.Running;
            }
            return State.Running;
        }
        return State.Success;
    }
}
