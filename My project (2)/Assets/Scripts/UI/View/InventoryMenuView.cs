using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryMenuView : View
{
    [SerializeField] private Button inventoryButton;

    public override void Initialize()
    {
        inventoryButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }

    [SerializeField] private InventoryItem itemPrefab;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private List<InventoryItem> uiItemsList = new List<InventoryItem>();

    [SerializeField] private InventoryDescription itemDescription;

    public event Action<int> OnDescriptionRequested, OnItemActionRequested;

    private void Start()
    {
        itemDescription.ResetDescription();
    }

    public override void Show()
    {
        base.Show();
        Debug.Log("InventoryMenuView Show");

        ResetSelection();
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            InventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel);
            uiItemsList.Add(item);

            item.OnItemClicked += HandleItemSelection;
            item.OnRightClicked += HandleShowItemActions;
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (uiItemsList.Count > itemIndex) 
        {
            uiItemsList[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    public void ResetSelection()
    {
        itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (InventoryItem item in uiItemsList)
        {
            item.Deselect();
        }
    }

    private void HandleItemSelection(InventoryItem item)
    {
        int index = uiItemsList.IndexOf(item);

        if (index == -1)
            return;

        OnDescriptionRequested?.Invoke(index);
    }

    private void HandleShowItemActions(InventoryItem item)
    {
        
    }

    internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, int holdNumber,
        string description, string otherInfo)
    {
        itemDescription.SetDescription(itemImage, name, holdNumber, description, otherInfo);

        DeselectAllItems();
        uiItemsList[itemIndex].Select();
    }

    public void ResetAllItems()
    {
        foreach (var item in uiItemsList)
        {
            item.ResetData();
            item.Deselect();
        }
    }
}
