using AttributeSystem.Authoring;
using AttributeSystem.Components;
using System;
using TMPro;
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

    [SerializeField] private FitUIAttributeEventHandler UIattributeEventHandler;

    [SerializeField] private bool isHealthBar;

    [SerializeField] private TextMeshProUGUI nameText;

    public bool ShouldFaceToCamera = true;

    public bool ShouldChangeSize = false;

    private void OnEnable()
    {
        if (UIattributeEventHandler != null)
        {
            UIattributeEventHandler.OnUIMaxAttributeChanged += OnAttributeMaxValueChanged;
        }
            
    }

    private void OnDisable()
    {
        if (UIattributeEventHandler != null)
        {
            UIattributeEventHandler.OnUIMaxAttributeChanged -= OnAttributeMaxValueChanged;
        }
    }

    private void Start()
    {
        if (nameText != null)
        {
            nameText.text = attributeSystemComponent.gameObject.name;
        }
    }

    // 收到attribute的maxValue 更改ui components 去符合長度
    private void OnAttributeMaxValueChanged(float value)
    {
        if (ShouldChangeSize)
        {
            var rect = this.gameObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
        }
        
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

        if (ShouldFaceToCamera)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
