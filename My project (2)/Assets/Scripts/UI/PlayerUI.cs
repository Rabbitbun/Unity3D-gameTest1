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
    [SerializeField, ReadOnly] public AbilitySystem abilitySystem;
    [SerializeField, ReadOnly] public StatusSystem statusSystem;


    // 技能圖示Textures
    [SerializeField, ReadOnly] public List<Texture2D> Textures;
    // 實際技能圖示
    [SerializeField, ReadOnly] public List<Image> Icons;
    // 技能圖示遮罩
    [SerializeField] private List<Image> _iconMasks;
    // 冷卻時間顯示
    [SerializeField] private List<Text> _coolDownTexts;
    [SerializeField] public List<TextMeshProUGUI> CoolDowntextMeshPros;

    // 生命條顯示
    public HealthBar healthBar;

    public List<GameObject> SkillUiBtn;

    // 全部可使用的技能
    public List<GameObject> Skills;
    // 目前要顯示的技能 (4個)
    public List<GameObject> CurrentSkills;
    // 全部可使用的技能的scriptableObject
    public List<AbilityObject> abilityObjects;
    // all skills cooldown scripts
    [SerializeField] private List<AbilityCooldown> _skillsCooldownScript;
    // 當前可使用技能的 skills cooldown scripts
    public List<AbilityCooldown> currentSkillsCoolDownScript;

    private void Awake()
    {
        
    }

    private void Start()
    {
        // 先抓取玩家的system
        abilitySystem = UIManager.Instance.abilitySystem;
        statusSystem = UIManager.Instance.statusSystem;
        // 複製一份技能 gameObject
        Skills = abilitySystem.AbilityObjectList;

        // 複製一份 scriptable objs
        for (int i = 0; i < abilitySystem.AbilityObjectList.Count; i++)
        {
            abilityObjects.Add(abilitySystem.AbilityObjectList[i].GetComponent<Spell>().spellObj);
        }

        healthBar.Setup(statusSystem.healthSystem);

        InitPlayerUI();
    }

    public void InitPlayerUI()
    {
        _skillsCooldownScript = new List<AbilityCooldown>();
        currentSkillsCoolDownScript = new List<AbilityCooldown>();
        _coolDownTexts = new List<Text>();
        CoolDowntextMeshPros = new List<TextMeshProUGUI>();
        Textures = new List<Texture2D>();
        Icons = new List<Image>();

        // 全部技能
        for (int i = 0; i < Skills.Count; i++)
        {
            // 填充冷卻腳本
            _skillsCooldownScript.Add(Skills[i].GetComponent<AbilityCooldown>());
            // 初始化冷卻腳本
            _skillsCooldownScript[i].Initialize();
            // 初始化texture2d
            Textures.Add(abilityObjects[i].texture);
        }
        //  當前會顯示的UI
        for (int i = 0; i < 4; i++)
        {
            //CurrentSkills[i] = Skills[i];
            CurrentSkills.Add(Skills[i]);

            currentSkillsCoolDownScript.Add(_skillsCooldownScript[i]);

            CoolDowntextMeshPros.Add(SkillUiBtn[i].GetComponentInChildren<TextMeshProUGUI>());
            CoolDowntextMeshPros[i].text = currentSkillsCoolDownScript[i].CoolDownTimeLeft.ToString();
            //CoolDowntextMeshPros[i].enabled = false;
            //_coolDownTexts[i].text = CoolDowntextMeshPros[i].text;
        }
        TextureToIcon();
    }


    private void TextureToIcon()
    {
        for (int i = 0; i < 4; i++)
        {
            Sprite sprite = Sprite.Create(Textures[i], new Rect(0, 0, Textures[i].width, Textures[i].height), Vector2.zero);
            Icons.Add(_iconMasks[i]);
            Icons[i].sprite = sprite;
        }
    }
}


