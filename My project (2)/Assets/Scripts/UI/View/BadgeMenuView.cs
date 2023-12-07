using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgeMenuView : View
{
    [SerializeField] private Button badgeButton;

    public override void Initialize()
    {
        badgeButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
