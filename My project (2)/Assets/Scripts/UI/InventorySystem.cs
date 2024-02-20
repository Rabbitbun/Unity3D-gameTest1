using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] private InventoryMenuView inventoryMenu;

    [SerializeField] private InventoryData inventoryData;

    [SerializeField] private bool IsUpdateIventory;

    private void Start()
    {
        PrepareUI();
    }

    private void Update()
    {
        IsUpdateIventory = MasterManager.Instance.GameEventManager.IsgamePaused;

        if (IsUpdateIventory)
        {
            if (inventoryData != null)
            {
                foreach (var item in inventoryData.GetCurrentInventoryState())
                {
                    inventoryMenu.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                }
            }
            
        }
    }
    private void PrepareUI()
    {
        inventoryMenu.InitializeInventoryUI(inventoryData.Size);

        this.inventoryMenu.OnDescriptionRequested += HandleDescriptionRequest;
        this.inventoryMenu.OnItemActionRequested += HandleItemActionRequest;
    }

    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItemStruct inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        ItemData item = inventoryItem.item;
        inventoryMenu.UpdateDescription(itemIndex, item.ItemImage, 
            item.name, inventoryItem.quantity, item.Description, item.OtherInfo);
    }

    private void HandleItemActionRequest(int itemIndex)
    {

    }
}
