using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapAndQusetMenuView : View
{
    [SerializeField] private Button mapAndQuestButton;

    public override void Initialize()
    {
        mapAndQuestButton.onClick.AddListener(() => ViewManager.Show(this, false));
    }
}
