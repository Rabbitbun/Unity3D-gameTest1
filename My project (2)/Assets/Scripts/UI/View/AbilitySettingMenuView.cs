using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySettingMenuView : View
{
    [SerializeField] private Button abilitySettingButton;

    public override void Initialize()
    {
        abilitySettingButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
