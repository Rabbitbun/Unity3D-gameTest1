using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;
using UnityEngine.AI;

public class RotateToTarget : ActionNode
{
    [SerializeField] private AbilitySystemCharacter target;
    public float rotationSpeed = 5f;
    protected override void OnStart() {
        this.target = blackboard.GetData<AbilitySystemCharacter>("target");    
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        Vector3 targetPosition = target.gameObject.transform.position;
        Vector3 direction = (targetPosition - context.transform.position).normalized;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            context.transform.rotation = Quaternion.Slerp(context.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        return State.Success;

    }
}
