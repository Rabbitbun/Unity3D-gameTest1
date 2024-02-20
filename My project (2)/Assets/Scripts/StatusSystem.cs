using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this System is for player
/// </summary>
public class StatusSystem : MonoBehaviour
{
    public GameObject HpBarPrefab;
    public HealthSystem healthSystem;
    public HealthBar healthBar;

    // basic status
    public float maxHp = 100;
    public float currentHp;
    public float maxMana = 100f;
    public float currentMana;
    public float manaRechargeRate = 5f;
    public float hpRechargeRate = 1f;
    private float nextHpRechargeTime = 0f;

    public SkillSystem SkillSystem;
    //[SerializeField] public UIManager uIMamager;

    public event System.EventHandler<StatusEventArgs> StatusChanging;

    private void Awake()
    {
        currentHp = maxHp;
        currentMana = maxMana;
        SkillSystem = GetComponent<SkillSystem>();
        healthSystem = new HealthSystem(maxHp);
    }

    void Start()
    {
        SkillSystem.StatusChanging += OnStatusChanging;
        this.StatusChanging += OnStatusChanging;
        //uIMamager.StatusChanging += OnStatusChanging;
    }

    private void OnStatusChanging(object sender, StatusEventArgs e)
    {
        if (e.target == "Player")
        {
            if (e.actType == StatusEventArgs.ActType.Damage)
            {
                currentHp -= e.value;
                //uIMamager.HealthChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Damage, "UI", e.value));
                UIManager.Instance.HealthChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Damage, "UI", e.value));
            }
            else if (e.actType == StatusEventArgs.ActType.Heal)
            {
                currentHp += e.value;
                //uIMamager.HealthChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Heal, "UI", e.value));
                UIManager.Instance.HealthChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Heal, "UI", e.value));
            }    
            else if (e.actType == StatusEventArgs.ActType.CostMana)
            {
                currentMana -= e.value;
            }
        }
    }

    void Update()
    {
        print("HP: " + currentHp);
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(11);
        }

        if (currentMana < maxMana)
        {
            currentMana += manaRechargeRate * Time.deltaTime;
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
        }
        if (Time.time > nextHpRechargeTime && currentHp < maxHp)    
        {
            print("NOW CAN RECOVER");
            TakeHeal(1);
            nextHpRechargeTime = Time.time + hpRechargeRate; // set next time to recharge hp
        }

    }

    public void TakeDamage(float damageAmount)
    {
        if (this.StatusChanging != null)
        {
            StatusChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Damage, "Player", damageAmount));
        }
    }

    public void TakeHeal(float healAmount)
    {
        if (this.StatusChanging != null)
        {
            StatusChanging(this, new StatusEventArgs(StatusEventArgs.ActType.Heal, "Player", healAmount));
        }
    }
}

public class StatusEventArgs : System.EventArgs
{
    public enum ActType
    {
        Damage,
        Heal,
        CostMana,
    }
    public ActType actType;
    public string target;
    public float value;

    public StatusEventArgs(ActType _actType, string _target, float _value)
    {
        actType = _actType;
        target = _target;
        value = _value;
    }
}
