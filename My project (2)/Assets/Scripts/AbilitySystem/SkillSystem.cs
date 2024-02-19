using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// have spell objects to spell, handle skill button、spell cast
/// </summary>
public class SkillSystem : MonoBehaviour
{
    public event System.EventHandler<StatusEventArgs> StatusChanging;

    //public ObjectPool<GameObject> AbilityPool; // 待處理
    
    // 法術物件變數
    public Spell spell;

    // 存放全部可使用的技能的預製物件(4+4個)
    public List<GameObject> SkillObjectList;
    // 目前一次可使用的技能預製物件(4個)
    public List<GameObject> CurrentSkillObjectList;

    // 拆組
    public Dictionary<int, List<GameObject>> SkillDict = new Dictionary<int, List<GameObject>>();

    int currentListIndex;
    // 有幾列
    int skillListNumber = 2;
    // 每列有幾個
    int numbersInList = 4;

    [SerializeField] bool CanReleaseSkill = false;
    int contentIndex = -1;

    // 技能物件施放起點位置
    [SerializeField] private Transform _castPoint;

    // 玩家狀態
    [ReadOnly] public StatusSystem statusSystem;

    [ReadOnly] public PlayerController playerController;

    private void Awake()
    {
        // 註冊輸入事件
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.UseSkill.performed += UseSkill;
        MasterManager.Instance.PlayerInputManager.PlayerInput.Player.SwitchSkill.performed += SwitchList;

        playerController = GetComponent<PlayerController>();

        // 將技能物件分組放入dictionary
        for (int i = 0; i < SkillObjectList.Count; i++)
        {
            // 4個一組
            int ListIndex = i / numbersInList;
            if (!SkillDict.ContainsKey(ListIndex))
            {
                SkillDict[ListIndex] = new List<GameObject>();
            }
            SkillDict[ListIndex].Add(SkillObjectList[i]);
        }

        CurrentSkillObjectList = SkillDict[0];

    }
    private void Start()
    {
        statusSystem = GetComponent<StatusSystem>();

        // 複製一份給物件池，讓他初始化
        //AbilityObjectPool.Instance.ObjectList = SkillObjectList;
        //AbilityObjectPool.Instance.InitPool();

        //UIManager.Instance.InitSkillListOnUI(CurrentSkillObjectList, currentListIndex);
    }

    private void Update()
    {
        // 是否可放出技能(有按下技能按鍵)
        if (CanReleaseSkill)
        {
            if (PlayerInputManager.Instance.RightClickPressed)
            {
                print("按下右鍵取消釋放...");
                CanReleaseSkill = false;
                PlayerInputManager.Instance.Aiming = false;
            }
            if (PlayerInputManager.Instance.leftClick)
            {
                print("按下左鍵釋放技能...");
                ReleaseSkill(contentIndex);
                CanReleaseSkill = false;
                PlayerInputManager.Instance.Aiming = false;
            }
        }
    }

    /// <summary>
    /// 施放技能
    /// </summary>
    /// <param name="index">技能欄位數字</param>
    private void CastSpell(int index)
    {
        Instantiate(CurrentSkillObjectList[index], _castPoint.position, _castPoint.rotation);

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
    /// <param name="index">技能欄位數字</param>
    /// <returns></returns>
    private bool HasEnoughMana(int index)
    {
        spell = CurrentSkillObjectList[index].GetComponent<Spell>();
        return statusSystem.currentMana - spell.spellObj.ManaCost >= 0f;
    }

    private void ReleaseSkill(int index)
    {
        // index : 0 1 2 3
        //int index = int.Parse(context.control.name) - 1;

        // send message to UIManager, so that it can handle UI, need to check cooldown and mana cost
        if (!UIManager.Instance.playerUI.IsSkillOnCooldown(index) && HasEnoughMana(index))
        {
            // cast the spell
            CastSpell(index);

            switch ((index + 1).ToString())
            {
                // change UI
                case "1":
                    UIManager.Instance.OnSkillButtonPressed(1);
                    print("技能槽位1被施放了!");
                    break;
                case "2":
                    UIManager.Instance.OnSkillButtonPressed(2);
                    print("技能槽位2被施放了!");
                    break;
                case "3":
                    UIManager.Instance.OnSkillButtonPressed(3);
                    print("技能槽位3被施放了!");
                    break;
                case "4":
                    UIManager.Instance.OnSkillButtonPressed(4);
                    print("技能槽位4被施放了!");
                    break;
            }
        }
        // in cooldown or has not enough mana
        else
        {
            print("技能冷卻中/沒有魔力了...");
        }
    }

    /// <summary>
    /// catch input to send what ability be used
    /// 按下技能按鈕後 進入瞄準模式 在按下攻擊鍵後才能夠施放技能
    /// </summary>
    private void UseSkill(InputAction.CallbackContext context)
    {
        if (MasterManager.Instance.GameEventManager.IsgamePaused == true)
            return;
        if (CanReleaseSkill)
            return;

        CanReleaseSkill = true;
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
        //                UIManager.Instance.OnSkillButtonPressed(1);
        //                print("技能槽位1被施放了!");
        //                break;
        //            case "2":
        //                UIManager.Instance.OnSkillButtonPressed(2);
        //                print("技能槽位2被施放了!");
        //                break;
        //            case "3":
        //                UIManager.Instance.OnSkillButtonPressed(3);
        //                print("技能槽位3被施放了!");
        //                break;
        //            case "4":
        //                UIManager.Instance.OnSkillButtonPressed(4);
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
        currentListIndex = ++currentListIndex % skillListNumber;

        // CurrentSkillObjectList.Clear();
        //for (int i = currentListIndex * 4, j = 0; i < currentListIndex * 4 + 4; i++, j++)
        //{
        //    CurrentSkillObjectList[j] = SkillObjectList[i];
        //}
        CurrentSkillObjectList = SkillDict[currentListIndex];
        UIManager.Instance.InitSkillListOnUI(CurrentSkillObjectList, currentListIndex);

    }
    void CostMana(float costAmount)
    {
        if (this.StatusChanging != null)
        {
            StatusChanging(this, new StatusEventArgs(StatusEventArgs.ActType.CostMana, "Player", costAmount));
        }
    }

}
