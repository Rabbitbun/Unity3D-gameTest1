using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public Transform pfHealthBar;
    HealthSystem healthSystem;

    void Start()
    {
        healthSystem = new HealthSystem(100);

        Transform healthBarTransform = Instantiate(pfHealthBar, Vector3.zero, Quaternion.identity);
        HealthBar healthBar = healthBarTransform.GetComponent<HealthBar>();
        healthBar.Setup(healthSystem);
        //healthBarTransform.parent = this.transform;
        healthBarTransform.SetParent(transform, false);
        healthBarTransform.localPosition = new Vector3(0, 1, 0);
    }

    void Update()
    {
        
    }

    public void Damage(float damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }
}
