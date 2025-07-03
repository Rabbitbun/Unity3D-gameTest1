using UnityEngine;
using TheKiwiCoder;
using AbilitySystem.Authoring;

public class UseAbility : ActionNode
{
    public AbstractAbilityScriptableObject ability;
    private AbstractAbilitySpec abilitySpec;

    public float WaitTimeAfterAbility = 0;
    private float waitStartTime;

    [Tooltip("若設為true, WaitTimeAfterAbility會被替換成隨機值")]
    public bool setRandom = false;
    public float maxValue = 5.0f;
    public float minValue = 1.0f;

    private bool isAbilityInProgress;
    private bool abilityActivated;
    private bool abilityFailed;
    private bool abilityDeActivated;

    protected override void OnStart() {
        Debug.Log("進入使用能力節點......");
        this.context.abilitySystemCharacter.OnGameplayAbilityActivated += OnAbilityActivated;
        this.context.abilitySystemCharacter.OnGameplayAbilityFailedActivation += OnAbilityFailed;
        this.context.abilitySystemCharacter.OnGameplayAbilityDeactivated += OnAbilityDeActivated;

        isAbilityInProgress = false;
        abilityActivated = false;
        abilityFailed = false;

        GrantCastableAbilities();

        if (setRandom)
            WaitTimeAfterAbility = Random.Range(minValue, maxValue);
    }

    protected override void OnStop()
    {
        this.context.abilitySystemCharacter.OnGameplayAbilityActivated -= OnAbilityActivated;
        this.context.abilitySystemCharacter.OnGameplayAbilityFailedActivation -= OnAbilityFailed;
        this.context.abilitySystemCharacter.OnGameplayAbilityDeactivated -= OnAbilityDeActivated;
    }

    protected override State OnUpdate()
    {
        if (!isAbilityInProgress)
        {
            isAbilityInProgress = true;
            TryUseAbility();
            return State.Running;
        }

        if (abilityFailed)
        {
            return State.Failure;
        }

        if (abilityActivated)
        {
            return State.Running;
        }

        if (abilityDeActivated)
        {
            if (Time.time - waitStartTime > WaitTimeAfterAbility)
                return State.Success;
            return State.Running;
        }

        return State.Running;
    }

    private void OnAbilityFailed(AbstractAbilitySpec spec, string abilityName, ActivationFailure reason)
    {
        abilityFailed = true;
    }

    private void OnAbilityActivated(AbstractAbilitySpec spec, string abilityName)
    {
        abilityActivated = true;
    }
    private void OnAbilityDeActivated(AbstractAbilitySpec spec, string abilityName)
    {
        abilityDeActivated = true;
        waitStartTime = Time.time;
    }

    // create ability spec
    void GrantCastableAbilities()
    {
        var spec = ability.CreateSpec(this.context.abilitySystemCharacter);
        this.context.abilitySystemCharacter.GrantAbility(spec);
        this.abilitySpec = spec;
    }

    void TryUseAbility()
    {
        this.context.abilitySystemCharacter.UseAbilitySpec(this.abilitySpec);
    }

}
