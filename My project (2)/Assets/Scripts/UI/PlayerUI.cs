using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This contains HP bar、skills、
/// </summary>
public class PlayerUI : MonoBehaviour
{
    [SerializeField, ReadOnly] public AbilitySystem abilitySystem;
    [SerializeField, ReadOnly] public StatusSystem statusSystem;
    //skills
    //[SerializeField] public Texture2D Texture;
    //[SerializeField] public Image Icon;
    //[SerializeField] private Image _iconMask;
    //[SerializeField] private Text _coolDownText;
    //[SerializeField] public TextMeshProUGUI CoolDowntextMeshPro;

    [SerializeField] public List<Texture2D> Textures;
    [SerializeField] public List<Image> Icons;
    [SerializeField] private List<Image> _iconMasks;
    [SerializeField] private List<Text> _coolDownTexts;
    [SerializeField] public List<TextMeshProUGUI> CoolDowntextMeshPros;

    public HealthBar healthBar;

    public List<GameObject> Skills;
    public List<GameObject> SkillsUI;
    public List<AbilityObject> abilityObjects;
    // all skills cooldown scripts
    private List<AbilityCooldown> _skillsCooldown;
    // current style skills cooldown scripts
    private List<AbilityCooldown> _currentSkillsCoolDown;

    private void Awake()
    {
        
        
    }

    private void Start()
    {
        abilitySystem = UIManager.Instance.abilitySystem;
        statusSystem = UIManager.Instance.statusSystem;

        // 全部的可使用技能
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
        _skillsCooldown = new List<AbilityCooldown>();
        _currentSkillsCoolDown = new List<AbilityCooldown>(); 
        CoolDowntextMeshPros = new List<TextMeshProUGUI>();
        Textures = new List<Texture2D>();
        Icons = new List<Image>();


        for (int i = 0; i < Skills.Count; i++)
        {
            _skillsCooldown.Add(Skills[i].GetComponent<AbilityCooldown>());
        }
        for (int i = 0; i < 4; i++)
        {
            SkillsUI[i] = Skills[i];

            _currentSkillsCoolDown.Add(_skillsCooldown[i]);

            CoolDowntextMeshPros.Add(SkillsUI[i].GetComponent<TextMeshProUGUI>());
            //_coolDownTexts[i].text = CoolDowntextMeshPros[i].text;
            
        }

        for (int i = 0; i < Skills.Count; i++)
        {
            //Textures[i] = abilityObjects[i].texture;
            Textures.Add(abilityObjects[i].texture);
        }

        for (int i = 0; i < 4; i++)
        {
            //_iconMasks[i] = abilityObjects[i].darkMask;
            
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


