using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This contains HP bar、ability
/// </summary>
public class PlayerUI : MonoBehaviour
{
    // 從player上抓取system
    [SerializeField, ReadOnly] public SkillSystem SkillSystem;
    [SerializeField, ReadOnly] public StatusSystem StatusSystem;
    // PlayerUI上的技能冷卻系統腳本
    [SerializeField, ReadOnly] public SkillCooldown SkillCooldownScript;
    // 技能圖示Textures
    [SerializeField, ReadOnly] public List<Texture2D> Textures;
    // 實際技能圖示
    [SerializeField] public List<Image> Icons;
    
    // 技能圖示遮罩
    [SerializeField] private List<Image> _iconMasks;
    // 冷卻時間文字顯示
    [SerializeField] public List<TextMeshProUGUI> CoolDowntextMeshPros;

    // 生命條顯示
    public HealthBar healthBar;
    // 技能欄位按鈕
    public List<GameObject> SkillUiBtn;

    // 全部可使用的技能
    public List<GameObject> Skills;
    // 目前要顯示的技能 (4個)
    public List<GameObject> CurrentSkills;
    // 全部可使用的技能的scriptableObject
    public List<SkillObject> SkillObjects;
    // 目前要顯示的技能圖示
    public List<Image> currentIcons;

    private int _listIndex = 0;

    // all skills cooldown scripts
    //[SerializeField] private List<AbilityCooldown> _skillsCooldownScript;
    //// 當前可使用技能的 skills cooldown scripts
    //public List<AbilityCooldown> currentSkillsCoolDownScript;

    private void Awake()
    {
        
    }

    private void Start()
    {
        //// 先抓取玩家的system
        //SkillSystem = UIManager.Instance.SkillSystem;
        //StatusSystem = UIManager.Instance.StatusSystem;

        SkillCooldownScript = GetComponent<SkillCooldown>();

        // 複製一份技能 gameObject
        Skills = SkillSystem.SkillObjectList;

        // 複製一份 scriptable objs
        for (int i = 0; i < SkillSystem.SkillObjectList.Count; i++)
        {
            SkillObjects.Add(SkillSystem.SkillObjectList[i].GetComponent<Spell>().spellObj);
        }

        healthBar.Setup(StatusSystem.healthSystem);

        InitPlayerUI();
    }

    public void InitPlayerUI()
    {
        CoolDowntextMeshPros = new List<TextMeshProUGUI>();
        Textures = new List<Texture2D>();
        //Icons = new List<Image>();

        SkillCooldownScript.Initialize(SkillObjects);

        // 全部技能
        for (int i = 0; i < Skills.Count; i++)
        {
            // 填充冷卻腳本
            //_skillsCooldownScript.Add(Skills[i].GetComponent<AbilityCooldown>());
            
            // 初始化冷卻腳本
            //_skillsCooldownScript[i].Initialize();

            // 初始化texture2d
            Textures.Add(SkillObjects[i].texture);
        }
        

        //  當前會顯示的UI
        for (int i = 0; i < 4; i++)
        {
            CurrentSkills.Add(Skills[i]);
            //Icons.Add(_iconMasks[i]);
            //currentSkillsCoolDownScript.Add(_skillsCooldownScript[i]);

            CoolDowntextMeshPros.Add(SkillUiBtn[i].GetComponentInChildren<TextMeshProUGUI>());
            CoolDowntextMeshPros[i].text = "";
            //CoolDowntextMeshPros[i].enabled = false;
            //_coolDownTexts[i].text = CoolDowntextMeshPros[i].text;
        }
        TextureToIcon();
    }

    private void Update()
    {
        // 更新技能按鈕上的冷卻文字
        for (int i = 0, j = _listIndex*4; i < 4; i++, j++)
        {
            CoolDowntextMeshPros[i].text = SkillCooldownScript.coolDownTimeLefts[j].ToString("0");
            if (CoolDowntextMeshPros[i].text == "0") CoolDowntextMeshPros[i].enabled = false;
            else CoolDowntextMeshPros[i].enabled = true;

            
            _iconMasks[i].fillAmount = Mathf.Lerp(0, 1,
                                        SkillCooldownScript.coolDownTimeLefts[j]
                                        / SkillCooldownScript.coolDownDurations[j]);
        }   
    }

    private void TextureToIcon()
    {
        for (int i = 0, j = _listIndex*4; i < 4; i++, j++)
        {
            Sprite sprite = Sprite.Create(Textures[j], new Rect(0, 0, Textures[j].width, Textures[j].height), Vector2.zero);
            Icons[i].sprite = sprite;
        }
    }
    /// <summary>
    /// 技能按鈕觸發，以技能id來看
    /// </summary>
    /// <param name="index">第幾個按鈕 0 1 2 3</param>
    public void SkillBtnTriggered(int index)
    {
        SkillObject ao = CurrentSkills[index].GetComponent<Spell>().spellObj;
        int id = ao.ID;
        float timeLeft = ao.Cooldown;
        SkillCooldownScript.coolDownTimeLefts[id-1] += timeLeft;
    }

    public bool IsSkillOnCooldown(int BtnIndex)
    {
        SkillObject ao = CurrentSkills[BtnIndex].GetComponent<Spell>().spellObj;
        int id = ao.ID;
        return SkillCooldownScript.IsOnCooldown[id - 1];
    }

    public void ChangeSkillList(int ListIndex)
    {
        _listIndex = ListIndex;

        for (int i = 0, j = _listIndex*4; i < 4; i++, j++)
        {
            CurrentSkills[i] = Skills[j];
        }
        TextureToIcon();
    }
}


