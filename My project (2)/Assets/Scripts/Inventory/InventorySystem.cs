using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// 擔任InventoryUI跟背後邏輯的溝通
/// </summary>
public class InventorySystem : MonoBehaviour
{
    // inventory UI
    [SerializeField] private InventoryMenuView inventoryMenu;

    // inventory data (scriptable object)
    [SerializeField] private InventoryData inventoryData;

    [SerializeField] private bool IsUpdateIventory;

    // 初始化的list
    public List<InventoryItemStruct> InitialItems = new List<InventoryItemStruct>();

    private void Start()
    {
        PrepareUI();
        PreperInventoryData();
    }

    private void Update()
    {
        // 檢查是否需要更新Inventory (打開菜單時更新)
        IsUpdateIventory = MasterManager.Instance.GameEventManager.IsgamePaused;

        if (IsUpdateIventory)
        {
            foreach (var item in inventoryData.GetCurrentInventoryState())
            {
                inventoryMenu.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void PrepareUI()
    {
        inventoryMenu.InitializeInventoryUI(inventoryData.Size);

        this.inventoryMenu.OnDescriptionRequested += HandleDescriptionRequest;
        this.inventoryMenu.OnItemActionRequested += HandleItemActionRequest;
    }

    /// <summary>
    /// 初始化data
    /// </summary>
    private void PreperInventoryData()
    {
        inventoryData.Initialize();
        inventoryData.OnInventoryUpdated += UpdateInventoryUI;
        foreach (InventoryItemStruct item in InitialItems)
        {
            if (item.IsEmpty)
                continue;

            inventoryData.AddItem(item);
        }
    }

    private void UpdateInventoryUI(Dictionary<int, InventoryItemStruct> inventoryState)
    {
        inventoryMenu.ResetAllItems();
        foreach (var item in inventoryState)
        {
            inventoryMenu.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
        }
    }

    /// <summary>
    /// 處理描述區塊，選擇可以顯示描述，點選空白處初始化(不顯示任何描述)
    /// </summary>
    /// <param name="itemIndex">被選取的物品</param>
    private void HandleDescriptionRequest(int itemIndex)
    {
        InventoryItemStruct inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
        {
            inventoryMenu.ResetSelection();
            return;
        }

        ItemData item = inventoryItem.item;
        string otherInfo = PrepareDescription(inventoryItem);
        inventoryMenu.UpdateDescription(itemIndex, item.ItemImage, 
            item.name, inventoryItem.quantity, item.Description, otherInfo);
    }

    private string PrepareDescription(InventoryItemStruct inventoryItem)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(inventoryItem.item.Description);
        sb.AppendLine();
        for (int i = 0; i < inventoryItem.itemState.Count; i++) 
        {
            sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName} " +
                    $": {inventoryItem.itemState[i].value} / " +
                    $"{inventoryItem.item.DefaultParametersList[i].value}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private void HandleItemActionRequest(int itemIndex)
    {
        InventoryItemStruct inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        IItemAction itemAction = inventoryItem.item as IItemAction;
        if (itemAction != null) 
        {
            inventoryMenu.ShowItemAction(itemIndex);
            inventoryMenu.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
        }

        IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            inventoryMenu.AddAction("Drop", () => DropItem(itemIndex, inventoryItem.quantity));
        }
    }

    private void DropItem(int itemIndex, int quantity)
    {
        inventoryData.RemoveItem(itemIndex, quantity);
        inventoryMenu.ResetSelection();
    }

    public void PerformAction(int itemIndex)
    {
        InventoryItemStruct inventoryItem = inventoryData.GetItemAt(itemIndex);
        if (inventoryItem.IsEmpty)
            return;

        IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
        if (destroyableItem != null)
        {
            inventoryData.RemoveItem(itemIndex, 1);
        }

        IItemAction itemAction = inventoryItem.item as IItemAction;
        if (itemAction != null)
        {
            itemAction.PerformAction(gameObject, inventoryItem.itemState);

            if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                inventoryMenu.ResetSelection();
        }
    }

}
