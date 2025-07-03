using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;

public class VisionCheck : ActionNode
{
    public float viewDistance = 20f; // 距離
    public float viewAngle = 45f; // 角度
    public int rayCount = 10;
    public LayerMask obstacleMask;

    public float CheckTime = 0.5f; // 頻率
    private float checkTimer = 0f;

    private Transform targetTransform;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        checkTimer += Time.deltaTime;
        if (checkTimer > CheckTime) 
        {
            checkTimer = 0f;

            if (GetPlayerInVision())
            {
                return State.Success;
            }
            //else
            //{
            //    return State.Failure;
            //}
        }
        return State.Success;
        //return State.Running;
    }

    private bool GetPlayerInVision()
    {
        Transform self = this.context.transform;
        GameObject target = blackboard.GetData<AbilitySystemCharacter>("target").gameObject;
        //blackboard.UpdateData<Vector3>("moveToPosition", target.transform.position);
        //GameObject target = blackboard.target.gameObject;

        RaycastHit hit;
        Vector3 rayDir = target.transform.position - self.position;
        rayDir.y = 0;

        var angle = Vector3.Angle(rayDir, self.forward);

        //Debug.Log(angle);

        if (Vector3.Angle(rayDir, self.forward) < viewAngle)
        {
            if (Physics.Raycast(self.position + new Vector3(0, 1, 0), rayDir, out hit, viewDistance))
            {
                Debug.DrawRay(self.position + new Vector3(0, 1, 0), rayDir * hit.distance, Color.red, 0.5f);

                if (hit.collider.gameObject == target)
                {
                    Debug.Log("檢測到玩家!!!" + "  " + target.name + " " + hit);
                    //blackboard.UpdateData<bool>("PauseCheck", true);
                    blackboard.UpdateData<AIState>("AIState", AIState.Combat);
                    blackboard.UpdateData<AICombatState>("AICombatState", AICombatState.Observed);
                    //blackboard.UpdateData<Vector3>("moveToPosition", hit.collider.gameObject.transform.position);
                    //blackboard.IsPlayerNearby = true;
                    //blackboard.moveToPosition = hit.collider.gameObject.transform.position;
                    return true;
                }
            }
        }
        return false;
    }

}
