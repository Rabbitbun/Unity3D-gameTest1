using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuView : View
{
    [SerializeField] private Button inventoryButton;

    public override void Initialize()
    {
        inventoryButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
