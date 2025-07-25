using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ItemType
{
    Consumable,
    Equipment,
    // 其他類型...
}

public abstract class ItemData : ScriptableObject
{
    [field: SerializeField] public bool IsStackable { get; set; }
    [field: SerializeField] public int MaxStackSize { get; set; } = 1;
    [field: SerializeField] public string ItemName{ get; set; }
    [field: SerializeField] public int ID { get; set; }
    [field: SerializeField] public Sprite ItemImage { get; set; }
    [field: SerializeField, TextArea] public string Description { get; set; }
    [field: SerializeField, TextArea] public string OtherInfo { get; set; }

    public ItemType itemType;

    [field: SerializeField] public List<ItemParameter> DefaultParametersList { get; set; }

}

[Serializable]
public struct ItemParameter : IEquatable<ItemParameter>
{
    public ItemParameterSO itemParameter;
    public float value;

    public bool Equals(ItemParameter other)
    {
        return other.itemParameter == itemParameter;
    }
}
