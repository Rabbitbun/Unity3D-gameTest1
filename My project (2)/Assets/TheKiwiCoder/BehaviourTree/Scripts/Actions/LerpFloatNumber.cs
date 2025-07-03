using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class LerpFloatNumber : ActionNode
{
    public string paraName;
    public float startValue;
    public float endValue;
    
    public float duration; // 持續時間參數
    [SerializeField, ReadOnly] private float elapsedTime; // 跟蹤進度

    protected override void OnStart() {
        context.animator.SetFloat(paraName, startValue);
        elapsedTime = 0;
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //float currValue = context.animator.GetFloat(paraName);
        float currValue = blackboard.GetData<float>(paraName);
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        t = Mathf.Clamp01(t); // 0~1

        float newValue = Mathf.Lerp(currValue, endValue, t);
        blackboard.UpdateData<float>(paraName, newValue);
        context.animator.SetFloat(paraName, newValue);

        //var value = context.animator.GetFloat(paraName);
        //if (startValue >= endValeu) // 例如start:2 end:1
        //{
        //    context.animator.SetFloat(paraName, Mathf.Lerp(value, endValeu, Time.deltaTime));
        //}
        //else
        //{
        //    if (value <= startValue)
        //        value = startValue;
        //    context.animator.SetFloat(paraName, Mathf.Lerp(value, endValeu, Time.deltaTime));
        //}

        ////// 检查是否完成插值
        //if (t >= 1f)
        //{
        //    return State.Success;
        //}
        //else
        //{
        //    return State.Running;
        //}



        return State.Success;
    }
}
