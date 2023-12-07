using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuView : View
{
    [SerializeField] private Button settingButton;

    public override void Initialize()
    {
        settingButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
