using DG.Tweening;
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
    [SerializeField] private float switchDuration = 0.5f;

    [SerializeField] private InputReader _inputReader = default;

    private int _activePanelIndex = 0; // Track the currently active panel

    private void OnEnable()
    {
        _inputReader.switchAbilityListEvent += SwitchPanelActiveHandler;
    }

    private void OnDisable()
    {
        _inputReader.switchAbilityListEvent -= SwitchPanelActiveHandler;
    }

    public void SwitchPanelActiveHandler()
    {
        // Toggle active panel index
        _activePanelIndex = (_activePanelIndex + 1) % 2;

        if (_activePanelIndex == 0)
        {
            SwitchPanel(abilityPanel2, abilityPanel1);
        }
        else
        {
            SwitchPanel(abilityPanel1, abilityPanel2);
        }

        ResetAllIcons();
        FadeOutInactivePanelIcons();
    }

    /// <summary>
    /// Swaps the positions of two panels with animation.
    /// </summary>
    private void SwitchPanel(RectTransform inactivePanel, RectTransform activePanel)
    {
        // Use DOTween to animate the position swap
        Vector2 tempPosition = activePanel.anchoredPosition;

        activePanel.DOAnchorPos(inactivePanel.anchoredPosition, switchDuration).SetEase(Ease.InOutQuad);
        inactivePanel.DOAnchorPos(tempPosition, switchDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            inactivePanel.gameObject.SetActive(false);
            activePanel.gameObject.SetActive(true);
        });
    }

    /// <summary>
    /// Fades out the icons of the inactive panel with animation.
    /// </summary>
    private void FadeOutInactivePanelIcons()
    {
        if (_activePanelIndex == 0)
        {
            FadeOutPanelIcons(icons2, abilityPanel2);
            FadeInPanelIcons(icons1);
        }
        else
        {
            FadeOutPanelIcons(icons1, abilityPanel1);
            FadeInPanelIcons(icons2);
        }
    }

    private void FadeOutPanelIcons(List<Image> icons, RectTransform panel)
    {
        foreach (Image icon in icons)
        {
            icon.DOFade(0f, switchDuration).SetEase(Ease.InOutQuad);
        }
        panel.GetComponent<CanvasGroup>().DOFade(0f, switchDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
        });
    }

    private void FadeInPanelIcons(List<Image> icons)
    {
        foreach (Image icon in icons)
        {
            icon.DOFade(1.0f, switchDuration).SetEase(Ease.InOutQuad);
        }
    }

    private void ResetAllIcons()
    {
        foreach (Image icon in icons1)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1.0f);
        }
        foreach (Image icon in icons2)
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, 1.0f);
        }
    }
}
