using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EdibleItemData : ItemData, IDestroyableItem, IItemAction
{
    [SerializeField] private List<ModifierData> modifierDatas = new List<ModifierData>();

    public string ActionName => "Consume";

    public AudioClip actionSFX {get; private set; }

    public bool PerformAction(GameObject character)
    {
        foreach (ModifierData data in modifierDatas)
        {
            data.stateModifier.AffectCharacter(character, data.value);
        }
        return true;
    }
}


public interface IDestroyableItem
{

}

public interface IItemAction
{
    public string ActionName { get; }
    public AudioClip actionSFX { get; } // 不必要
    bool PerformAction(GameObject character);
}

[System.Serializable]
public class ModifierData
{
    public CharacterStateModifierSO stateModifier;
    public float value;
}