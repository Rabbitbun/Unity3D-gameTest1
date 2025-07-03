using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TheKiwiCoder;
using AttributeSystem.Authoring;
using System.Threading.Tasks;
using System;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private AbstractAbilityScriptableObject initialStats;

    [SerializeField]
    private AbilitySystemCharacter asc;

    [SerializeField]
    private AttributeScriptableObject HealthAttribute;

    [SerializeField]
    public BehaviourTreeRunner behaviourRunner;

    public Blackboard blackboard;

    [Header("Sensors")]
    [SerializeField]
    private PlayerSensor FollowPlayerSensor;
    [SerializeField]
    private PlayerSensor MeleePlayerSensor;
    [SerializeField]
    private PlayerSensor ObservedPlayerSensor;
    [SerializeField]
    private PlayerSensor FarAttackPlayerSensor;
    [SerializeField]
    private PlayerSensor MidAttackPlayerSensor;
    [SerializeField]
    private PlayerSensor CloseAttackPlayerSensor;

    [Space]
    [Header("Debug Info")]
    [SerializeField]
    public bool IsDead;
    [SerializeField]
    private bool IsInMeleeRange;
    [SerializeField]
    private bool IsInChaseRange;
    [SerializeField]
    private bool IsInObservedRange;
    [SerializeField]
    private bool IsInFarDistanceRange;
    [SerializeField]
    private bool IsInMiddleDistanceRange;
    [SerializeField]
    private bool IsInCloseDistanceRange;

    private Animator Animator;
    private NavMeshAgent Agent;

    public GameObject FelledMessage;

    private void Awake()
    {
        asc = GetComponent<AbilitySystemCharacter>();
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        behaviourRunner = GetComponent<BehaviourTreeRunner>();
    }

    void Start()
    {
        var spec = initialStats.CreateSpec(asc);
        asc.GrantAbility(spec);
        StartCoroutine(spec.TryActivateAbility());

        FollowPlayerSensor.OnPlayerEnter += FollowPlayerSensor_OnPlayerEnter;
        FollowPlayerSensor.OnPlayerExit += FollowPlayerSensor_OnPlayerExit;
        MeleePlayerSensor.OnPlayerEnter += MeleePlayerSensor_OnPlayerEnter;
        MeleePlayerSensor.OnPlayerExit += MeleePlayerSensor_OnPlayerExit;
        ObservedPlayerSensor.OnPlayerEnter += ObservedPlayerSensor_OnPlayerEnter;
        ObservedPlayerSensor.OnPlayerExit += ObservedPlayerSensor_OnPlayerExit;

        FarAttackPlayerSensor.OnPlayerEnter += FarAttackPlayerSensor_OnPlayerEnter;
        FarAttackPlayerSensor.OnPlayerExit += FarAttackPlayerSensor_OnPlayerExit;
        MidAttackPlayerSensor.OnPlayerEnter += MidAttackPlayerSensor_OnPlayerEnter;
        MidAttackPlayerSensor.OnPlayerExit += MidAttackPlayerSensor_OnPlayerExit;
        CloseAttackPlayerSensor.OnPlayerEnter += CloseAttackPlayerSensor_OnPlayerEnter;
        CloseAttackPlayerSensor.OnPlayerExit += CloseAttackPlayerSensor_OnPlayerExit;

        blackboard = behaviourRunner.tree.blackboard;
    }

    async void Update()
    {
        if (IsDead)
        {
            return;
        }

        if (IsInChaseRange)
        {
            asc.AttributeSystem.GetAttributeValue(HealthAttribute, out var health);
            if (health.BaseValue <= 0)
            {
                behaviourRunner.Stop = true;
                Animator.Play("Idle");
                Animator.SetTrigger("Dead");
                IsDead = true;

                //StartCoroutine(showDieMessage());
                FelledMessage.SetActive(true);
                await Task.Delay(TimeSpan.FromSeconds(5f));
                FelledMessage.SetActive(false);
            }
        }
        

        if (behaviourRunner == null) return;

        var isAttacking = blackboard.GetData<bool>("UsingAbility");

        // 當不在攻擊的時候才進行sensor的判定更新
        if (isAttacking == false)
        {
            blackboard.UpdateData<bool>("IsInChaseRange", IsInChaseRange);
            blackboard.UpdateData<bool>("IsInMeleeRange", IsInMeleeRange);
            blackboard.UpdateData<bool>("IsInObservedRange", IsInObservedRange);

            blackboard.UpdateData<bool>("IsInFarDistanceRange", IsInFarDistanceRange);
            blackboard.UpdateData<bool>("IsInMiddleDistanceRange", IsInMiddleDistanceRange);
            blackboard.UpdateData<bool>("IsInCloseDistanceRange", IsInCloseDistanceRange);
        }

    }

    private IEnumerator showDieMessage()
    {
        FelledMessage.SetActive(true);
        yield return new WaitForSeconds(5f);
        FelledMessage.SetActive(false);
        yield break;
    }

    private void FollowPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInChaseRange = false;

    private void FollowPlayerSensor_OnPlayerEnter(Transform Player) => IsInChaseRange = true;

    private void MeleePlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInMeleeRange = false;

    private void MeleePlayerSensor_OnPlayerEnter(Transform Player) => IsInMeleeRange = true;

    private void ObservedPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInObservedRange = false;

    private void ObservedPlayerSensor_OnPlayerEnter(Transform Player) => IsInObservedRange = true;

    private void FarAttackPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInFarDistanceRange = false;

    private void FarAttackPlayerSensor_OnPlayerEnter(Transform Player) => IsInFarDistanceRange = true;

    private void MidAttackPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInMiddleDistanceRange = false;

    private void MidAttackPlayerSensor_OnPlayerEnter(Transform Player) => IsInMiddleDistanceRange = true;

    private void CloseAttackPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition) => IsInCloseDistanceRange = false;

    private void CloseAttackPlayerSensor_OnPlayerEnter(Transform Player) => IsInCloseDistanceRange = true;

}
