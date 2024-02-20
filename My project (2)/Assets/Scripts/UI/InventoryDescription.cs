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

    [SerializeField] private int holdNumber;

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
        this.holdNumber = 0;
        this.description.text = "";
        this.otherInfo.text = "";
    }

    public void SetDescription(Sprite sprite, string itemTitle, int holdNumber, 
        string itemDescription, string itemOtherInfo)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.title.text = itemTitle;
        //this.info.text = itemInfo;
        this.holdNumber = holdNumber;
        this.info.text = holdNumber + " / 99";
        this.description.text = itemDescription;
        this.otherInfo.text = itemOtherInfo;
    }
}
