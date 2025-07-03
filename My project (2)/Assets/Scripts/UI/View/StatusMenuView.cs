using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilitySystem;
using AttributeSystem.Components;
using TMPro;

public class StatusMenuView : View
{
    [SerializeField] private Button statusButton;

    public override void Initialize()
    {
        statusButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }

    [SerializeField, ReadOnly] private AbilitySystemCharacter Player;
    [SerializeField, ReadOnly] private AttributeSystemComponent AttributeSystem;

    [SerializeField] private TextMeshProUGUI[] Texts;

    private void Start()
    {
        
    }

    public override void Show()
    {
        base.Show();
        Debug.Log("StatusMenuView Show");

        UpdateShowingData();
    }

    private void UpdateShowingData()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player").GetComponent<AbilitySystemCharacter>();
            AttributeSystem = Player.GetComponent<AttributeSystemComponent>();
        }

        var attributeCache = AttributeSystem.mAttributeIndexCache;
        int i = 0;

        foreach (var attributeEntry in attributeCache)
        {
            var attributeKey = attributeEntry.Key;
            int attributeIndex = attributeEntry.Value;

            if (AttributeSystem.GetAttributeValue(attributeKey, out var value))
            {
                Debug.Log($"Attribute: {attributeKey.name}, Value: {value.CurrentValue}");
            }
            else
            {
                Debug.Log($"Attribute: {attributeKey.name} not found.");
            }

            Texts[i].text = $"{attributeKey.name}: {value.CurrentValue}";
            i += 1;
        }

    }
}
