using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterStatHealthModifierSO : CharacterStateModifierSO
{
    public override void AffectCharacter(GameObject character, float val)
    {
        StatusSystem statusSystem = character.GetComponent<StatusSystem>();
        if (statusSystem != null)
            statusSystem.TakeHeal(val);
    }
}
