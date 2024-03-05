using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class InventoryData : ScriptableObject
{
    [field: SerializeField] private List<InventoryItemStruct> inventoryItems;

    [field: SerializeField] public int Size { get; private set; } = 10;

    public event Action<Dictionary<int, InventoryItemStruct>> OnInventoryUpdated;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItemStruct>();
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItemStruct.GetEmptyItem());
        }
    }

    public int AddItem(ItemData item, int quantity, List<ItemParameter> itemState = null)
    {
        if (item.IsStackable == false)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                while (IsInventoryFull(item) == false && quantity > 0)
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                }
                InformAboutChange();
                return quantity;
            }
        }

        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }
    public void AddItem(InventoryItemStruct item)
    {
        AddItem(item.item, item.quantity);
    }

    private int AddItemToFirstFreeSlot(ItemData item, int quantity, List<ItemParameter> itemState = null)
    {
        InventoryItemStruct newItem = new InventoryItemStruct
        {
            item = item,
            quantity = quantity,
            itemState = new List<ItemParameter>(itemState == null ? item.DefaultParametersList :  itemState)
        };

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    /// <summary>
    /// 檢查是否滿了 如果滿了就回傳true(有空位等於false)
    /// </summary>
    /// <returns></returns>
    private bool IsInventoryFull(ItemData itemdata)
    {
        if (itemdata.IsStackable == false)
            return false;

        foreach (var item in inventoryItems)
        {
            if (item.IsEmpty)
                continue;

            if (item.item.ID == itemdata.ID)
                return true;
        }

        return false;
    }
        //=>inventoryItems.Where(item => item.IsEmpty).Any() == false;

    private int AddStackableItem(ItemData item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
                continue;

            if (inventoryItems[i].item.ID == item.ID)
            {
                int amountPossibleToTake = inventoryItems[i].item.MaxStackSize - inventoryItems[i].quantity;

                if (amountPossibleToTake == 0)
                    return quantity;

                if (quantity > amountPossibleToTake)
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(inventoryItems[i].item.MaxStackSize);
                    quantity -= amountPossibleToTake;
                }
                else // quantity <= amountPossibleToTake
                {
                    inventoryItems[i] = inventoryItems[i].ChangeQuantity(
                        inventoryItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }
        while (quantity > 0 && IsInventoryFull(item) == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }
        return quantity;
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (inventoryItems.Count > itemIndex)
        {
            if (inventoryItems[itemIndex].IsEmpty)
                return;

            int reminder = inventoryItems[itemIndex].quantity - amount;
            if (reminder <= 0) 
            {
                // 清空
                inventoryItems[itemIndex] = InventoryItemStruct.GetEmptyItem();
            }
            else // reminder > 0
            {
                // 用剩餘的數量更新
                inventoryItems[itemIndex] = inventoryItems[itemIndex].ChangeQuantity(reminder);
            }
            InformAboutChange();
        }
    }

    public Dictionary<int, InventoryItemStruct> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItemStruct> returnValue = new Dictionary<int, InventoryItemStruct>();
        
        for (int i = 0; i < inventoryItems.Count; i++) 
        {
            if (inventoryItems[i].IsEmpty)
                continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue;
    }

    public InventoryItemStruct GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    public void InformAboutChange()
    {
        OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
    }

    
}

[System.Serializable]
public struct InventoryItemStruct
{
    public int quantity;
    public ItemData item;
    public List<ItemParameter> itemState;
    public bool IsEmpty => item == null;

    public InventoryItemStruct ChangeQuantity(int newQuantity)
    {
        return new InventoryItemStruct
        {
            item = this.item,
            quantity = newQuantity,
            itemState = new List<ItemParameter>(this.itemState),
        };
    }

    public static InventoryItemStruct GetEmptyItem()
        => new InventoryItemStruct
        {
            item = null,
            quantity = 0,
            itemState = new List<ItemParameter>(),
        };
}