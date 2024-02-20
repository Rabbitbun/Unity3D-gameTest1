using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentWeapon : MonoBehaviour
{
    [SerializeField] private EquippableItemData weapon;

    [SerializeField] private InventoryData inventoryData;

    [SerializeField] private List<ItemParameter> parametersToModify, itemCurrentState;

    public void SetWeapon(EquippableItemData weaponItemData, List<ItemParameter> itemState)
    {
        if (weapon != null)
        {
            inventoryData.AddItem(weapon, 1, itemCurrentState);
        }

        this.weapon = weaponItemData;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        ModifyParameters();
    }

    /// <summary>
    /// 修改參數 例如耐久度減少等等
    /// </summary>
    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
    }
}
