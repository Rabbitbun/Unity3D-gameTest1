using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryData : ScriptableObject
{
    [field: SerializeField] private List<InventoryItemStruct> inventoryItems;

    [field: SerializeField] public int Size { get; private set; } = 10;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItemStruct>();
        for (int i = 0; i < Size; i++)
        {
            inventoryItems.Add(InventoryItemStruct.GetEmptyItem());
        }
    }

    public void AddItem(ItemData item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].IsEmpty)
            {
                inventoryItems[i] = new InventoryItemStruct
                {
                    item = item,
                    quantity = quantity
                };
            }
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
}

[System.Serializable]
public struct InventoryItemStruct
{
    public int quantity;
    public ItemData item;
    public bool IsEmpty => item == null;

    public InventoryItemStruct ChangeQuantity(int newQuantity)
    {
        return new InventoryItemStruct
        {
            item = this.item,
            quantity = newQuantity,
        };
    }

    public static InventoryItemStruct GetEmptyItem()
        => new InventoryItemStruct
        {
            item = null,
            quantity = 0,
        };
}