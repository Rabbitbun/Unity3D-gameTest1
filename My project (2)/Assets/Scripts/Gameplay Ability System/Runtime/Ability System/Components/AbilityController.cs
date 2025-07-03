using UnityEngine;
using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine.UI;


public class AbilityController : MonoBehaviour
{
    // 所有可用的 Abilities
    public AbstractAbilityScriptableObject[] Abilities;

    // 初始化狀態用的 Abilities
    public AbstractAbilityScriptableObject[] InitialisationAbilities;

    // 第0或1個List 用來計算 要使用0~3或是4~7的技能
    [SerializeField] public int currentListsIndex { get; private set; } = 0;

    private AbilitySystemCharacter abilitySystemCharacter;

    private AbstractAbilitySpec[] abilitySpecs;

    [SerializeField] private InputReader _inputReader = default;
    //public PlayerInputManager _inputReader;

    [ReadOnly] public int CurrentUsedAbilityIndex = -1;

    //public Image[] Cooldowns;

    public Text text;

    private void OnEnable()
    {
        //_inputReader = PlayerInputManager.Instance;
        _inputReader.attackEvent += HandleAbilityUse;
        //_inputReader.StopattackEvent += HandleAbilityUse;

        _inputReader.startedChanting += HandleAbilityUse;
        _inputReader.stoppedChanting += HandleAbilityUse;

        _inputReader.dogeRollEvent += HandleAbilityUse;
        _inputReader.guardEvent += HandleAbilityUse;
        _inputReader.useItemEvent += HandleAbilityUse;


        _inputReader.useAbility1Event += HandleAbilityUse;
        _inputReader.useAbility2Event += HandleAbilityUse;
        _inputReader.useAbility3Event += HandleAbilityUse;
        _inputReader.useAbility4Event += HandleAbilityUse;

        _inputReader.StopuseAbility2Event += HandleAbilityUse;

        _inputReader.switchAbilityListEvent += HandleSwitchAbilityListChanged;
    }

    private void OnDisable()
    {
        _inputReader.attackEvent -= HandleAbilityUse;
        //_inputReader.StopattackEvent -= HandleAbilityUse;

        _inputReader.startedChanting -= HandleAbilityUse;
        _inputReader.stoppedChanting -= HandleAbilityUse;

        _inputReader.dogeRollEvent -= HandleAbilityUse;
        _inputReader.guardEvent -= HandleAbilityUse;
        _inputReader.useItemEvent -= HandleAbilityUse;

        _inputReader.useAbility1Event -= HandleAbilityUse;
        _inputReader.useAbility2Event -= HandleAbilityUse;
        _inputReader.useAbility3Event -= HandleAbilityUse;
        _inputReader.useAbility4Event -= HandleAbilityUse;

        _inputReader.StopuseAbility2Event -= HandleAbilityUse;

        _inputReader.switchAbilityListEvent -= HandleSwitchAbilityListChanged;
    }

    private void OnDestroy()
    {
        foreach (var spec in abilitySpecs)
        {
            if (spec != null)
            {
                
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        //AbstractAbilitySpec spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
        //this.abilitySystemCharacter.GrantAbility(spec);

        ActivateInitialisationAbilities();
        GrantCastableAbilities();

        //text = GetComponent<Text>();
        abilitySystemCharacter.OnTagsChanged += (x, y, z, w) => UpdateUI();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        //if (_inputReader.IsChanting)
        //{
        //    UseAbility(8);
        //}
        if (CurrentUsedAbilityIndex >= 0)
        {
            UseAbility(CurrentUsedAbilityIndex);
        }
    }

    public void UpdateUI()
    {
        //TAGS
        if (text != null)
        {
            text.text = "";
            text.text += $"\n Tags:\n<color=#F5FF40>";
            foreach (var tag in this.abilitySystemCharacter.Tags)
            {
                text.text += tag.name + " ";
            }
            text.text += "</color>";
        }
        

    }

    // 啟用初始化ability
    void ActivateInitialisationAbilities()
    {
        for (var i = 0; i < InitialisationAbilities.Length; i++)
        {
            var spec = InitialisationAbilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            StartCoroutine(spec.TryActivateAbility());
        }
    }

    // spec所有的abilities
    void GrantCastableAbilities()
    {
        this.abilitySpecs = new AbstractAbilitySpec[Abilities.Length];
        for (var i = 0; i < Abilities.Length; i++)
        {
            if (Abilities[i] == null) continue;
            var spec = Abilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            this.abilitySpecs[i] = spec;
        }
    }

    private void HandleSwitchAbilityListChanged()
    {
        // 0 or 1
        currentListsIndex = (currentListsIndex + 1) % 2;
    }

    private void HandleAbilityUse(int index)
    {
        ///TODO: 可在技能SO上設定是否可以持續按住，若可以再進行，不行就直接使用。
        ///TODO: 額外寫GA的設定SO

        // ability 基礎技能(攻擊、防禦等等)為欄位 0 ~ 9 (10個)、特殊技能為欄位 10 ~ 17 (8個)
        // 基礎: 詠唱(1)、使用道具(2個)、普通攻擊(1個)、防禦(1個)、躲避(1個)、

        // 0: 左鍵(普通攻擊)、1: 右鍵(詠唱)、2: (Space)躲避、3: (LCtrl)防禦、4: (R)補血道具、

        if (index == -1)
        {
            CurrentUsedAbilityIndex = -1;
        }
        else if (index != -1 && index <= 9) // 0 ~ 9
        {
            // 詠唱(1)、普通攻擊(0)、防禦(3)，需要按住
            switch (index)
            {
                case 1:case 3:
                    CurrentUsedAbilityIndex = index;
                    break;
                case 0:case 2:case 4:case 5:case 6:
                case 7:case 8:case 9:
                    CurrentUsedAbilityIndex = -1;
                    UseAbility(index);
                    break;
            }
        }
        else
        {
            // 會傳入 ability: 10 11 12 13
            // index 會等於 this.currentListsIndex * 4 + index 也就是 0+(10~13) ~ 4+(10~13)
            CurrentUsedAbilityIndex = this.currentListsIndex * 4 + index;
        }
            
    }

    public void UseAbility(int i)
    {
        Debug.Log("Use Ability " + i);
        var spec = abilitySpecs[i];
        StartCoroutine(spec.TryActivateAbility());
    }

}
