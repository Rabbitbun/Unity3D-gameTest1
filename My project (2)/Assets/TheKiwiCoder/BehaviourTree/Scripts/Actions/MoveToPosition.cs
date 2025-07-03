using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;

public class MoveToPosition : ActionNode
{
    public float speed = 1;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;
    public float tolerance = 1.0f;

    protected override void OnStart() {
        context.agent.stoppingDistance = stoppingDistance;
        context.agent.speed = speed;
        context.agent.destination = blackboard.GetData<AbilitySystemCharacter>("target").transform.position;
        context.agent.updateRotation = updateRotation;
        context.agent.acceleration = acceleration;
    }

    protected override void OnStop() {
        //Debug.Log("MoveToPosition Node 暫停....");
        //context.agent.SetDestination(context.transform.position);
        context.agent.speed = 0;
        context.agent.ResetPath();
    }

    protected override State OnUpdate() {
        var targetPos = blackboard.GetData<AbilitySystemCharacter>("target").transform.position;
        context.agent.destination = targetPos;
        //context.agent.SetDestination(targetPos);

        if (context.agent.pathPending) {
            return State.Running;
        }

        if (context.agent.remainingDistance < tolerance) {
            return State.Success;
        }

        if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
            return State.Failure;
        }

        return State.Running;
    }
}
