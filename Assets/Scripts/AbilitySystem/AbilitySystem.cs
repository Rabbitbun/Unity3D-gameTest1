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

    //public ObjectPool<GameObject> AbilityPool; // 待處理
    
    // 法術物件變數
    [HideInInspector] public Spell spell;

    // 存放總共可使用的技能預製物件(4+4個)
    [SerializeField] public List<GameObject> AbilityObjectList;
    // 目前所可使用的技能預製物件(4個)
    [SerializeField] public List<GameObject> currentAbilityList;

    public Dictionary<int, List<GameObject>> AbilityDict = new Dictionary<int, List<GameObject>>();

    int currentLListIndex;
    int abilityListNumber = 2;
    int NumberInList = 4;

    [SerializeField] bool CanReleaseAbility = false;
    int contentIndex = -1;

    [SerializeField] private Transform _castPoint;

    // 玩家狀態
    [ReadOnly] public StatusSystem statusSystem;

    public PlayerController playerController;

    private void Awake()
    {
        // 註冊輸入事件
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.UseAbility.performed += UseAbility;
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.SwitchAbility.performed += SwitchList;

        playerController = GetComponent<PlayerController>();

        for (int i = 0; i < AbilityObjectList.Count; i++)
        {
            // 4個一組
            int ListIndex = i / NumberInList;
            if (!AbilityDict.ContainsKey(ListIndex))
            {
                AbilityDict[ListIndex] = new List<GameObject>();
            }
            AbilityDict[ListIndex].Add(AbilityObjectList[i]);
        }
        currentAbilityList = AbilityDict[0];

    }
    private void Start()
    {
        statusSystem = GetComponent<StatusSystem>();

        // 複製一份給物件池，讓他初始化
        //AbilityObjectPool.Instance.ObjectList = AbilityObjectList;
        //AbilityObjectPool.Instance.InitPool();

        //UIManager.Instance.InitAbilityOnUI(currentAbilityList, currentLListIndex);
    }

    private void Update()
    {
        if (CanReleaseAbility)
        {
            if (PlayerInputManager.Instance.RightClickPressed)
            {
                print("按下右鍵取消釋放...");
                CanReleaseAbility = false;
                PlayerInputManager.Instance.Aiming = false;
            }
            if (PlayerInputManager.Instance.leftClick)
            {
                print("按下左鍵取消釋放...");
                ReleaseAbility(contentIndex);
                CanReleaseAbility = false;
                PlayerInputManager.Instance.Aiming = false;
            }
        }
    }

    /// <summary>
    /// 施放技能
    /// </summary>
    /// <param name="index"></param>
    private void CastSpell(int index)
    {
        Instantiate(currentAbilityList[index], _castPoint.position, _castPoint.rotation);

        // 技能物件池的使用
        //AbilityObjectPool.Instance.GetIndex = index;
        //ObjectPool<GameObject> objectPool = AbilityObjectPool.Instance._objectPools[index];
        //GameObject obj = objectPool.Get();
        //obj.transform.position = _castPoint.position;
        //obj.transform.rotation = _castPoint.rotation;
        //objectPool.Release(obj);

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

    private void ReleaseAbility(int index)
    {
        // index : 0 1 2 3
        //int index = int.Parse(context.control.name) - 1;
        // send message to UIManager, so that it can deal with UI, need to check cooldown and mana
        if (!UIManager.Instance.playerUI.IsSkillOnCooldown(index) && HasEnoughMana(index))
        {
            // cast the spell
            CastSpell(index);

            switch ((index + 1).ToString())
            {
                // change UI
                case "1":
                    UIManager.Instance.OnAbilityButtonPressed(1);
                    print("技能槽位1被施放了!");
                    break;
                case "2":
                    UIManager.Instance.OnAbilityButtonPressed(2);
                    print("技能槽位2被施放了!");
                    break;
                case "3":
                    UIManager.Instance.OnAbilityButtonPressed(3);
                    print("技能槽位3被施放了!");
                    break;
                case "4":
                    UIManager.Instance.OnAbilityButtonPressed(4);
                    print("技能槽位4被施放了!");
                    break;
            }
        }
        else
        {
            print("技能冷卻中/沒有魔力了...");
        }
    }

    /// <summary>
    /// catch input to send what ability be used
    /// 按下技能按鈕後 進入瞄準模式 在按下攻擊鍵後才能夠施放技能
    /// </summary>
    private void UseAbility(InputAction.CallbackContext context)
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused == true)
            return;
        if (CanReleaseAbility) 
            return;

        CanReleaseAbility = true;
        PlayerInputManager.Instance.Aiming = true;
        contentIndex = int.Parse(context.control.name) - 1;
        //if (context.performed)
        //{
        //    // index : 0 1 2 3
        //    int index = int.Parse(context.control.name) - 1;
        //    // send message to UIManager, so that it can deal with UI, need to check cooldown and mana
        //    if (!UIManager.Instance.playerUI.IsSkillOnCooldown(index) && HasEnoughMana(index))
        //    {
        //        // cast the spell
        //        CastSpell(int.Parse(context.control.name) - 1);

        //        switch (context.control.name)
        //        {
        //            // change UI
        //            case "1":
        //                UIManager.Instance.OnAbilityButtonPressed(1);
        //                print("技能槽位1被施放了!");
        //                break;
        //            case "2":
        //                UIManager.Instance.OnAbilityButtonPressed(2);
        //                print("技能槽位2被施放了!");
        //                break;
        //            case "3":
        //                UIManager.Instance.OnAbilityButtonPressed(3);
        //                print("技能槽位3被施放了!");
        //                break;
        //            case "4":
        //                UIManager.Instance.OnAbilityButtonPressed(4);
        //                print("技能槽位4被施放了!");
        //                break;
        //        }

        //    }
        //    else
        //    {
        //        print("技能冷卻中/沒有魔力了...");
        //    }
        //}
    }

    public void SwitchList(InputAction.CallbackContext context)
    {
        // 0 1
        currentLListIndex = ++currentLListIndex % abilityListNumber;

        // currentAbilityList.Clear();
        //for (int i = currentLListIndex * 4, j = 0; i < currentLListIndex * 4 + 4; i++, j++)
        //{
        //    currentAbilityList[j] = AbilityObjectList[i];
        //}
        currentAbilityList = AbilityDict[currentLListIndex];
        UIManager.Instance.InitAbilityOnUI(currentAbilityList, currentLListIndex);

    }
    void CostMana(float costAmount)
    {
        if (this.StatusChanging != null)
        {
            StatusChanging(this, new StatusEventArgs(StatusEventArgs.ActType.CostMana, "Player", costAmount));
        }
    }

}
