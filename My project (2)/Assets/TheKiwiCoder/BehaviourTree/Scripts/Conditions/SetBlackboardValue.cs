using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using AbilitySystem;

public class SetBlackboardValue : ConditionNode
{
    public enum BlackboardDataType
    {
        String,
        Int,
        Float,
        Bool,
        Vector3,
        AIState,
        AICombatState,
        AbilitySystemCharacter
    }

    [Tooltip("要修改的類別名稱")]
    public string key;
    public BlackboardDataType dataType;
    public string stringValue;
    public int intValue;
    public float floatValue;
    public bool boolValue;
    public Vector3 vector3Value;
    public AIState aiStateValue;
    public AICombatState aiCombatStateValue;
    public AbilitySystemCharacter abilitySystemCharacterValue;



    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    public override bool Check()
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("Blackboard key is null or empty!");
            return false;
        }

        switch (dataType)
        {
            case BlackboardDataType.String:
                blackboard.UpdateData<string>(key, stringValue);
                break;
            case BlackboardDataType.Int:
                blackboard.UpdateData<int>(key, intValue);
                break;
            case BlackboardDataType.Float:
                blackboard.UpdateData<float>(key, floatValue);
                break;
            case BlackboardDataType.Bool:
                blackboard.UpdateData<bool>(key, boolValue);
                break;
            case BlackboardDataType.Vector3:
                blackboard.UpdateData<Vector3>(key, vector3Value);
                break;
            case BlackboardDataType.AIState:
                blackboard.UpdateData<AIState>(key, aiStateValue);
                break;
            case BlackboardDataType.AICombatState:
                blackboard.UpdateData<AICombatState>(key, aiCombatStateValue);
                break;
            case BlackboardDataType.AbilitySystemCharacter:
                blackboard.UpdateData<AbilitySystemCharacter>(key, abilitySystemCharacterValue);
                break;
            default:
                Debug.LogError("Unsupported Blackboard data type!");
                return false;
        }

        // always
        return true;
    }
}
