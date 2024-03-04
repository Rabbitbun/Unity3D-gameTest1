using System;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Gameplay Ability System/Attribute Event Handler/Attribute Change Fit UI")]
public class FitUIAttributeEventHandler : AbstractAttributeEventHandler
{
    [SerializeField] private AttributeScriptableObject PrimaryAttributeMax;

    public event Action<float> OnUIMaxAttributeChanged;

    public override void PreAttributeChange(AttributeSystemComponent attributeSystem, List<AttributeValue> prevAttributeValues, ref List<AttributeValue> currentAttributeValues)
    {
        var attributeCacheDict = attributeSystem.mAttributeIndexCache;

        if (attributeCacheDict.TryGetValue(PrimaryAttributeMax, out var primaryAttributeIndex))
        {
            var prevValue = prevAttributeValues[primaryAttributeIndex].CurrentValue;
            var currentValue = currentAttributeValues[primaryAttributeIndex].CurrentValue;

            if (prevValue != currentValue)
            {
                OnUIMaxAttributeChanged?.Invoke(currentValue);
            }
        }
    }
}
