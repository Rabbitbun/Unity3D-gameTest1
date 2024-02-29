using AttributeSystem.Authoring;
using AttributeSystem.Components;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIAttributeUpdater : MonoBehaviour
{
    [SerializeField]
    private AttributeSystemComponent attributeSystemComponent;

    [SerializeField]
    private BaseAttributeUIComponent attributeUI;

    [SerializeField]
    private AttributeScriptableObject currentAttribute;

    [SerializeField]
    private AttributeScriptableObject maxAttribute;

    [SerializeField] private bool isHealthBar;

    private void Start()
    {
        if (isHealthBar)
            currentAttribute.PreAttributeChange += OnAttributeChanged;
    }

    private void OnAttributeChanged(object sender, EventArgs e)
    {
        Debug.Log("在UIupdater中 的 OnAttributeChanged觸發了!!!!");
    }

    void LateUpdate()
    {
        if (!attributeSystemComponent) return;
        if (!attributeUI) return;
        if (!currentAttribute) return;
        if (!maxAttribute) return;

        if (attributeSystemComponent.GetAttributeValue(currentAttribute, out var currentAttributeValue)
            && attributeSystemComponent.GetAttributeValue(maxAttribute, out var maxAttributeValue))
        {
            attributeUI.SetAttributeValue(currentAttributeValue.CurrentValue, maxAttributeValue.CurrentValue);
        }
    }
}
