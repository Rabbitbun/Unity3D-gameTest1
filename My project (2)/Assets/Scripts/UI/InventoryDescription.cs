using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryDescription : MonoBehaviour
{
    [SerializeField] private Image itemImage;

    [SerializeField] private TMP_Text title;

    [SerializeField] private TMP_Text info;

    [SerializeField] private TMP_Text description;

    [SerializeField] private TMP_Text otherInfo;

    private void Awake()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        this.itemImage.gameObject.SetActive(false);
        this.title.text = "";
        this.info.text = "";
        this.description.text = "";
        this.otherInfo.text = "";
    }

    public void SetDescription(Sprite sprite, string itemTitle, string itemInfo, 
        string itemDescription, string itemOtherInfo)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.title.text = itemTitle;
        this.info.text = itemInfo;
        this.description.text = itemDescription;
        this.otherInfo.text = itemOtherInfo;
    }
}
