using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;

    private void OnTriggerEnter(Collider collision)
    {
        LootItem lootItem = collision.GetComponent<LootItem>();

        if (lootItem != null)
        {
            int reminder = inventoryData.AddItem(lootItem.InventoryItem, lootItem.Quantity);
            if (reminder == 0)
            {
                lootItem.DestoryItem();
            }
            else
            {
                 lootItem.Quantity = reminder;
            }
        }
    }
}
