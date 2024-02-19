using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Consumable,
    Equipment,
    // 其他類型...
}

[CreateAssetMenu(fileName = "New Item", menuName = "itemData")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public int ID;
    public Sprite Icon;

    public ItemType itemType;

    [TextArea]
    public string description;
}
