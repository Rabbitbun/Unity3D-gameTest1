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
    [SerializeField] public List<Texture2D> Textures;
    // 實際技能圖示
    [SerializeField] public List<Image> Icons;
    // 技能圖示遮罩
    [SerializeField] private List<Image> _iconMasks;
    // 冷卻時間顯示
    [SerializeField] private List<Text> _coolDownTexts;
    [SerializeField] public List<TextMeshProUGUI> CoolDowntextMeshPros;

    // 生命條顯示
    public HealthBar healthBar;

    // 全部可使用的技能
    public List<GameObject> Skills;
    // 目前要顯示的技能 (4個)
    public List<GameObject> SkillsUI;
    // 全部可使用的技能的scriptableObject
    public List<AbilityObject> abilityObjects;
    // all skills cooldown scripts
    [SerializeField] private List<AbilityCooldown> _skillsCooldownScript;
    // 當前可使用技能的 skills cooldown scripts
    public List<AbilityCooldown> currentSkillsCoolDown;

    private void Awake()
    {
        
    }

    private void Start()
    {
        abilitySystem = UIManager.Instance.abilitySystem;
        statusSystem = UIManager.Instance.statusSystem;

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
        currentSkillsCoolDown = new List<AbilityCooldown>(); 
        //CoolDowntextMeshPros = new List<TextMeshProUGUI>();
        Textures = new List<Texture2D>();
        Icons = new List<Image>();

        // 全部技能
        for (int i = 0; i < Skills.Count; i++)
        {
            // 填充冷卻腳本
            _skillsCooldownScript.Add(Skills[i].GetComponent<AbilityCooldown>());
            // 初始化冷卻腳本
            _skillsCooldownScript[i].Initialize(abilityObjects[i]);
            _skillsCooldownScript[i].abilityObject = abilityObjects[i];
            Textures.Add(abilityObjects[i].texture);
        }

        for (int i = 0; i < 4; i++)
        {
            SkillsUI[i] = Skills[i];

            currentSkillsCoolDown.Add(_skillsCooldownScript[i]);

            //CoolDowntextMeshPros.Add(SkillsUI[i].GetComponent<TextMeshProUGUI>());
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


