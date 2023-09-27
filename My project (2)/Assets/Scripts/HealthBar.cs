using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private HealthSystem healthSystem;
    public Slider healthSlider;
    public Image foreGroundImage;
    public bool isEnemy = true;
    public bool isOrb = false;

    private Camera cameraToFollow;

    public void Awake()
    {
        cameraToFollow = Camera.main;
        if (isOrb)
        {
            healthSlider = GetComponent<Slider>();
        }
    }

    public void Setup(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;

        healthSystem.OnHealthChange += HealthSystem_OnHealthChange;
    }

    private void HealthSystem_OnHealthChange(object sender, System.EventArgs e)
    {
        if (!isOrb)
            foreGroundImage.fillAmount = healthSystem.GetHealthPercent();
        else if (isOrb)
        {
            healthSlider.value = healthSystem.GetHealthPercent();
        }
    }

    private void Update()
    {
        if (isEnemy)
            transform.rotation = Quaternion.LookRotation(transform.position - cameraToFollow.transform.position);
    }
}
