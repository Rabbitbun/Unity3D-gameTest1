using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour, IuseAbility
{
    public static UIManager Instance { get; private set;}

    [SerializeField, ReadOnly] public PlayerUI playerUI;

    // 從玩家身上抓取System
    [SerializeField, ReadOnly] public AbilitySystem abilitySystem;
    [SerializeField, ReadOnly] public StatusSystem statusSystem;

    //public List<GameObject> Skills;
    // all skills cooldown scripts
    //private List<AbilityCooldown> _skillsCooldown = new List<AbilityCooldown>();
    // current style skills cooldown scripts
    //private List<AbilityCooldown> _currentSkillsCoolDown = new List<AbilityCooldown>();


    private void Awake()
    {
        Instance = GetComponent<UIManager>();

        playerUI = GetComponentInChildren<PlayerUI>();

        abilitySystem = GameObject.FindWithTag("Player").GetComponent<AbilitySystem>();
        statusSystem = GameObject.FindWithTag("Player").GetComponent<StatusSystem>();
        
        // TODO: 需要重新架構整個cooldown 腳本 
        
        //foreach (var go in abilitySystem.AbilityObjectList)
        //{
        //    _skillsCooldown.Add(go.GetComponent<AbilityCooldown>());
        //}
        //for (int i = 0; i < 4; i++)
        //{
        //    _currentSkillsCoolDown[i] = _skillsCooldown[i];
        //}
    }

    private void Start()
    {
        //StatusChanging += HealthChanging;
        
        //healthBar.Setup(statusSystem.healthSystem);
    }

    public void InitAbilityOnUI(List<GameObject> abilityList, int styleIndex)
    {
        playerUI.ChangeStyle(styleIndex);
    //    for (int i = styleIndex * 4, j = 0; i < styleIndex * 4 + 4; i++, j++)
    //    {
    //        _currentSkillsCoolDown[j] = _skillsCooldown[i];
    //        //_currentSkillsCoolDown[j].abilityObject = abilityList[i].GetComponent<Spell>().spellObj;
    //        //_currentSkillsCoolDown[j].Initialize(abilityList[i].GetComponent<Spell>().spellObj);
    //        _currentSkillsCoolDown[j].Initialize(_currentSkillsCoolDown[i].gameObject.GetComponent<Spell>().spellObj);

    //    }
    }


    public void OnAbilityButtonPressed(int index)
    {
        // call cool down script

        switch (index)
        {
            case 1:
                Debug.Log("Pressed Ability_1 button.");
                //playerUI.currentSkillsCoolDownScript[0].ButtonTriggered();
                playerUI.AbilityBtnTriggered(0);
                break;

            case 2:
                Debug.Log("Pressed Ability_2 button.");
                //_skillsCooldown[1].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[1].ButtonTriggered();
                playerUI.AbilityBtnTriggered(1);
                break;
            case 3:
                Debug.Log("Pressed Ability_3 button.");
                //_skillsCooldown[2].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[2].ButtonTriggered();
                playerUI.AbilityBtnTriggered(2);
                break;
            case 4:
                Debug.Log("Pressed Ability_4 button.");
                //_skillsCooldown[3].ButtonTriggered();
                //_currentSkillsCoolDown[0].ButtonTriggered();
                //playerUI.currentSkillsCoolDownScript[3].ButtonTriggered();
                playerUI.AbilityBtnTriggered(3);
                break;
            default:
                Debug.Log("Pressed invalid button.");
                break;
        }

    }

    public void HealthChanging(object sender, StatusEventArgs e)
    {
        if (e.target == "UI")
        {
            if (e.actType == StatusEventArgs.ActType.Damage)
            {
                statusSystem.healthSystem.Damage(e.value);
            }
            else if (e.actType == StatusEventArgs.ActType.Heal)
            {
                statusSystem.healthSystem.Heal(e.value);
            }
        }
        
    }

}