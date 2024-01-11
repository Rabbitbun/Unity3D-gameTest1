using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentMenuView : View
{
    [SerializeField] private Button equipmentButton;

    public override void Initialize()
    {
        equipmentButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
