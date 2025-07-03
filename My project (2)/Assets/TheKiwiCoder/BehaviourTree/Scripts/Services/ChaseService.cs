using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ChaseService : ServiceNode
{
    public string paraName;
    public float startValue;
    public float endValue;

    public float Lerpduration; // 持續時間參數
    [SerializeField, ReadOnly] private float elapsedTime, t; // 跟蹤進度

    protected override void OnStart() {
        Debug.Log("開始追蹤服務........");
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        float currValue = context.animator.GetFloat(paraName);
        elapsedTime += Time.deltaTime;
        t = elapsedTime / Lerpduration;
        t = Mathf.Clamp01(t); // 0~1

        float newValue = Mathf.Lerp(currValue, endValue, t);
        context.animator.SetFloat(paraName, newValue);

        return base.OnUpdate();
    }
    protected override State OnServiceUpdate()
    {
        if (t >= 1f)
        {
            //blackboard.UpdateData<AICombatState>("AICombatState", AICombatState.Attack);
            Debug.Log("追蹤結束...");
            return State.Success;
        }

        return State.Running;
    }
}
