using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitylMenuView : View
{
    [SerializeField] private Button abilityButton;

    public override void Initialize()
    {
        abilityButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
