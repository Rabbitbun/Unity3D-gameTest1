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

    [SerializeField] private int inventorySize = 10;

    [SerializeField] private InventoryDescription itemDescription;

    // temporary variables
    public Sprite image;
    public int quantity;
    public string title, info, description, otherInfo;

    private void Start()
    {
        InitializeInventoryUI(inventorySize);

        itemDescription.ResetDescription();

        uiItemsList[0].SetData(image, quantity);
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

    private void HandleItemSelection(InventoryItem item)
    {
        itemDescription.SetDescription(image, title, info, description, otherInfo);
        uiItemsList[0].Select();
    }

    private void HandleShowItemActions(InventoryItem item)
    {
        
    }
}
