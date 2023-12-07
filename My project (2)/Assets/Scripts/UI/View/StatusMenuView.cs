using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusMenuView : View
{
    [SerializeField] private Button statusButton;

    public override void Initialize()
    {
        statusButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
