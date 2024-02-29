using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuView : View
{
    [SerializeField] private Button inventoryButton;

    public override void Initialize()
    {
        inventoryButton.onClick.AddListener(() => ViewManager.Show(this, true));
    }

    // 物品框底
    [SerializeField] private InventoryItem itemPrefab;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] private List<InventoryItem> uiItemsList = new List<InventoryItem>();

    [SerializeField] private InventoryDescription itemDescription;

    public event Action<int> OnDescriptionRequested, OnItemActionRequested;

    [SerializeField] private ItemActionPanel actionPanel;

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

    public override void Hide()
    {
        base.Hide();

        
    }

    public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            InventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(contentPanel);
            uiItemsList.Add(item);

            item.OnItemClicked += HandleShowItemActions;
            item.OnMouseEnter += HandleItemSelection;
        }
    }

    public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
    {
        if (uiItemsList.Count > itemIndex)
        {
            uiItemsList[itemIndex].SetData(itemImage, itemQuantity);
        }
    }

    public void AddAction(string actionName, Action performAction)
    {
        actionPanel.AddButton(actionName, performAction);
    }

    public void ShowItemAction(int itemIndex) 
    {
        actionPanel.Toggle(true);
        actionPanel.transform.position = uiItemsList[itemIndex].transform.position;
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

        actionPanel.Toggle(false);
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
        int index = uiItemsList.IndexOf(item);

        if (index == -1)
            return;

        OnItemActionRequested?.Invoke(index);
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
