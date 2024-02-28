using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 當在 Inventory View 時, 按下操控選單(例如右鍵)時會額外跳出的Panel, 可以有像是使用/丟棄/裝備等等選項可選
/// </summary>
public class ItemActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject buttonPrefab;

    /// <summary>
    /// 添加選項
    /// </summary>
    /// <param name="name">選項的名稱</param>
    /// <param name="onClickAction">點選時的Action</param>
    public void AddButton(string name, Action onClickAction)
    {
        GameObject button = Instantiate(buttonPrefab, transform);
        button.GetComponent<Button>().onClick.AddListener(() => onClickAction());
        button.GetComponentInChildren<TMPro.TMP_Text>().text = name;
    }

    /// <summary>
    /// 是否要顯示選項panel的buttons
    /// </summary>
    /// <param name="val"></param>
    public void Toggle(bool val)
    {
        if (val == true)
            RemoveOldButtons();
        gameObject.SetActive(val);
    }

    public void RemoveOldButtons()
    {
        foreach (Transform transformChildObjects in this.transform)
        {
            Destroy(transformChildObjects.gameObject);
        }
    }
}
