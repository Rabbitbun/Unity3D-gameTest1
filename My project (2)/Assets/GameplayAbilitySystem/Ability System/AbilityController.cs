using UnityEngine;
using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections.Generic;

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

    //public Image[] Cooldowns;

    private void OnEnable()
    {
        _inputReader.useAbility1Event += HandleAbilityUse;
        _inputReader.useAbility2Event += HandleAbilityUse;
        _inputReader.useAbility3Event += HandleAbilityUse;
        _inputReader.useAbility4Event += HandleAbilityUse;

        _inputReader.switchAbilityListEvent += HandleSwitchAbilityListChanged;
    }

    private void OnDisable()
    {
        _inputReader.useAbility1Event -= HandleAbilityUse;
        _inputReader.useAbility2Event -= HandleAbilityUse;
        _inputReader.useAbility3Event -= HandleAbilityUse;
        _inputReader.useAbility4Event -= HandleAbilityUse;

        _inputReader.switchAbilityListEvent -= HandleSwitchAbilityListChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        AbstractAbilitySpec spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
        this.abilitySystemCharacter.GrantAbility(spec);

        ActivateInitialisationAbilities();
        GrantCastableAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        if (_inputReader.IsChanting)
        {
            UseAbility(8);
        }
    }

    // 啟用初始化的ability
    void ActivateInitialisationAbilities()
    {
        for (var i = 0; i < InitialisationAbilities.Length; i++)
        {
            var spec = InitialisationAbilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            StartCoroutine(spec.TryActivateAbility());
        }
    }

    // spec所有可用的abilities
    void GrantCastableAbilities()
    {
        this.abilitySpecs = new AbstractAbilitySpec[Abilities.Length];
        for (var i = 0; i < Abilities.Length; i++)
        {
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
        // index: 0 ~ 3
        index = this.currentListsIndex * 4 + index;
        // index: 0+(0~3) or 4+(0~3)
        UseAbility(index);
    }

    public void UseAbility(int i)
    {
        Debug.Log("Use Ability " + i);
        var spec = abilitySpecs[i];
        StartCoroutine(spec.TryActivateAbility());
    }

}
