using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [SerializeField] private InventoryData inventoryData;

    [SerializeField] private InputReader _inputReader = default;

    private LootItem currentLootItem;

    [SerializeField] private GameObject PickupIcon;

    private void Start()
    {
        _inputReader.interactEvent += OnInteract;
    }

    private void OnDestroy()
    {
        _inputReader.interactEvent -= OnInteract;
    }

    private void OnInteract()
    {
        if (currentLootItem != null)
        {
            PickupIcon.SetActive(false);
            int reminder = inventoryData.AddItem(currentLootItem.InventoryItem, currentLootItem.Quantity);
            if (reminder == 0)
            {
                currentLootItem.DestoryItem();
                currentLootItem = null;
            }
            else
            {
                currentLootItem.Quantity = reminder;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        LootItem lootItem = collision.GetComponent<LootItem>();

        //if (lootItem != null)
        //{
        //    int reminder = inventoryData.AddItem(lootItem.InventoryItem, lootItem.Quantity);
        //    if (reminder == 0)
        //    {
        //        lootItem.DestoryItem();
        //    }
        //    else
        //    {
        //         lootItem.Quantity = reminder;
        //    }
        //}

        if (lootItem != null)
        {
            currentLootItem = lootItem;
            PickupIcon.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (currentLootItem != null && other.GetComponent<LootItem>() == currentLootItem)
        {
            currentLootItem = null;
            PickupIcon.SetActive(false);
        }
    }

}
