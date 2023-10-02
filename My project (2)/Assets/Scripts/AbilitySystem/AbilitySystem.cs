using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

/// <summary>
/// have spell objects to spell, deal with ability button、spell cast
/// </summary>
public class AbilitySystem : MonoBehaviour
{
    public event System.EventHandler<StatusEventArgs> StatusChanging;

    //public ObjectPool<GameObject> AbilityPool;
    
    // 法術物件變數
    [HideInInspector] public Spell spell;

    // 存放總共可使用的技能預製物件
    [SerializeField] public List<GameObject> AbilityObjectList;
    // 目前Style所可使用的技能預製物件
    [SerializeField] public List<GameObject> currentAbilityList;

    public Dictionary<int, List<GameObject>> StyleAbilityDict = new Dictionary<int, List<GameObject>>();

    int currentUseAbilityIndex;

    [SerializeField] private Transform _castPoint;

    [SerializeField] public UIManager uiMamager;

    [ReadOnly] public StatusSystem statusSystem;

    private void Awake()
    {
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.UseAbility.performed += UseAbility;
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.SwitchAbility.performed += SwitchStyle;

        for (int i = 0; i < AbilityObjectList.Count; i++)
        {
            int styleIndex = i / 4;
            if (!StyleAbilityDict.ContainsKey(styleIndex))
            {
                StyleAbilityDict[styleIndex] = new List<GameObject>();
            }
            StyleAbilityDict[styleIndex].Add(AbilityObjectList[i]);
        }
        currentAbilityList = StyleAbilityDict[0];

        //AbilityObjectPool.Instance.LoadToPool(StyleAbilityDict);
        AbilityObjectPool.Instance.ObjectList = AbilityObjectList;
        for (int i = 0; i < AbilityObjectList.Count; i++)
        {
            AbilityObjectPool.Instance.LoadToPool(AbilityObjectList[i]);
        }
    }
    private void Start()
    {
        statusSystem = GetComponent<StatusSystem>();

        int cnt = AbilityObjectPool.Instance._objectPools.Count;
        Debug.Log("AbilityObjectPool Cnt: " + cnt);

        UIManager.Instance.InitAbilityOnUI(currentAbilityList, currentUseAbilityIndex);
    }

    private void CastSpell(int index)
    {
        //Instantiate(currentAbilityList[index], _castPoint.position, _castPoint.rotation);
        //ObjectPool<GameObject> objectPool = AbilityObjectPool.Instance._objectPools[index];
        //GameObject obj = objectPool.Get();
        //objectPool.Release(obj);
        Debug.Log(AbilityObjectPool.Instance.Check1());
        CostMana(spell.spellObj.ManaCost);
    }

    /// <summary>
    /// Check current mana Enough or not, if enough return true.
    /// </summary>
    /// <param name="index">The ability that player used index</param>
    /// <returns></returns>
    private bool HasEnoughMana(int index)
    {
        spell = currentAbilityList[index].GetComponent<Spell>();
        return statusSystem.currentMana - spell.spellObj.ManaCost >= 0f;
    }

    /// <summary>
    /// catch input to send what ability i use
    /// </summary>
    private void UseAbility(InputAction.CallbackContext context)
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused == true)
            return;

        if (context.performed)
        {
            // index : 0 1 2 3
            int index = int.Parse(context.control.name) - 1;
            // send message to UIManager, so that it can deal with UI, need to check cooldown and mana
            if (!uiMamager.IsSkillOnCooldown(index) && HasEnoughMana(index))
            {
                // cast the spell
                CastSpell(int.Parse(context.control.name) - 1);

                switch (context.control.name)
                {
                    // change UI
                    case "1":
                        uiMamager.OnAbilityButtonPressed(1);
                        print("Skill_1");
                        break;
                    case "2":
                        uiMamager.OnAbilityButtonPressed(2);
                        print("Skill_2");
                        break;
                    case "3":
                        uiMamager.OnAbilityButtonPressed(3);
                        print("Skill_3");
                        break;
                    case "4":
                        uiMamager.OnAbilityButtonPressed(4);
                        print("Skill_4");
                        break;
                }

            }
            else
            {
                print("SKILL is on cooldown...");
            }
        }
    }

    void SwitchStyle(InputAction.CallbackContext context)
    {
        // 0 1 2 3 
        currentUseAbilityIndex = ++currentUseAbilityIndex % 4;

        // currentAbilityList.Clear();
        for (int i = currentUseAbilityIndex * 4, j = 0; i < currentUseAbilityIndex * 4 + 4; i++, j++)
        {
            currentAbilityList[j] = AbilityObjectList[i];
        }
        UIManager.Instance.InitAbilityOnUI(currentAbilityList, currentUseAbilityIndex);

    }
    void CostMana(float costAmount)
    {
        if (this.StatusChanging != null)
        {
            StatusChanging(this, new StatusEventArgs(StatusEventArgs.ActType.CostMana, "Player", costAmount));
        }
    }

}
