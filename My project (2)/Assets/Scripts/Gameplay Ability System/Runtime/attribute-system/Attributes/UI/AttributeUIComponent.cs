using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AttributeUIComponent : BaseAttributeUIComponent
{
    [SerializeField] private Slider slider;

    [SerializeField] private float lerpSpeed;

    //[SerializeField] private Slider shrinkSlider;
    [SerializeField] private Image shrinkBar;
    [SerializeField] private float targetValue;
    [SerializeField] private float fadeSpeed = 1.0f;
    //[SerializeField] private float waitForFade;

    [SerializeField] private bool shouldShrink;

    public override void SetAttributeValue(float currentValue, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = math.lerp(slider.value, currentValue, Time.deltaTime * lerpSpeed);
        

        // 處理需要 shrink 的 bar
        if (shouldShrink && maxValue != 0) 
        {
            if (shrinkBar.fillAmount != (currentValue / maxValue))
            {
                targetValue = currentValue / maxValue;
                if (targetValue >= shrinkBar.fillAmount)
                {
                    shrinkBar.fillAmount = math.lerp(shrinkBar.fillAmount, targetValue, Time.deltaTime * lerpSpeed);
                }
                else
                {
                    var offset = shrinkBar.fillAmount / targetValue;
                    shrinkBar.fillAmount = math.lerp(shrinkBar.fillAmount, targetValue, Time.deltaTime * (fadeSpeed + offset));
                }
                
            }
        }
    }

}
