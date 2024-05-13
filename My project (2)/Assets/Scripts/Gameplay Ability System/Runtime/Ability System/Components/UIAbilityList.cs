using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIAbilityList : MonoBehaviour
{
    [SerializeField] private RectTransform abilityPanel1;
    [SerializeField] private RectTransform abilityPanel2;

    [SerializeField] private List<Image> icons1;
    [SerializeField] private List<Image> icons2;

    [SerializeField] private float iconFadeValue = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchPanelActiveHandler(int index)
    {
        SwitchPanel(abilityPanel1, abilityPanel2);
        ResetAllIcons();
        switch (index) 
        {
            case 0:
                FadeOutPanelIcon(icons2);
                break;            
            case 1:
                FadeOutPanelIcon(icons1);
                break;
        }
    }
    /// <summary>
    /// 對調位置
    /// </summary>
    private void SwitchPanel(RectTransform p1, RectTransform p2)
    {
        Vector2 temp = p1.anchoredPosition;
        p1.anchoredPosition = p2.anchoredPosition;
        p2.anchoredPosition = temp;
    }

    private void FadeOutPanelIcon(List<Image> icons)
    {
        foreach (Image icon in icons) 
        {
            Color currentColor = icon.color;
            currentColor.a = iconFadeValue;
            icon.color = currentColor;
        }
    }

    private void ResetAllIcons()
    {
        for (int i = 0; i < icons1.Count; i++) 
        {
            Color c1 = icons1[i].color;
            c1.a = 1.0f;
            icons1[i].color = c1;

            Color c2 = icons2[i].color;
            c2.a = 1.0f;
            icons2[i].color = c2;
        }
    }
}
