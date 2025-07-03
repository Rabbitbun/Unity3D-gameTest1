using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class TriggerAnimation : ActionNode
{
    public string paraName;

    protected override void OnStart() {
        context.animator.SetTrigger(paraName);
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        return State.Success;
    }
}
