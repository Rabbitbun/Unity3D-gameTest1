using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private Image itemImage;

    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Image borderImage;

    public event Action<InventoryItem> OnItemClicked, OnMouseEnter;

    [SerializeField, ReadOnly] private bool empty = true;

    private void Awake()
    {
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        this.itemImage.gameObject.SetActive(false);
        this.empty = true;
    }

    public void Deselect()
    {
        this.borderImage.enabled = false;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.quantityText.text = quantity.ToString() + "";
        this.empty = false;
    }

    public void Select()
    {
        this.borderImage.enabled = true;
    }
    
    public void OnPointerClick(PointerEventData pointerData)
    {
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter?.Invoke(this);
    }
}
