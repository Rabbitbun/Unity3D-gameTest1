using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Dot Ability")]
    public class DotAbilitySO : AbstractAbilityScriptableObject
    {
        public GameplayEffectScriptableObject GameplayEffect; // Gameplay Effect to apply

        public string AnimationTriggerName;

        public float DelayForApplyGE;

        public GameObject AbilityPrefab;

        public AnimationCue AnimationCue;
        public VFXCue VFXCue;
        public VFXCue PreVFXCue;

        public float DamageInterval;
        public float DamageTime;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new DotAbilitySpec(this, owner);
            spec.Level = owner.Level;
            spec.AnimationTriggerName = this.AnimationTriggerName;
            spec.abilityPrefab = this.AbilityPrefab;
            spec.DelayForApplyGE = this.DelayForApplyGE;
            spec.damageInterval = this.DamageInterval;
            spec.totalDamageTime = this.DamageTime;
            spec.animationCue = this.AnimationCue;
            spec.vfxCue = this.VFXCue;
            spec.preVFXCue = this.PreVFXCue;

            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        [System.Serializable]
        public class DotAbilitySpec : AbstractAbilitySpec
        {
            private Animator animatorComponent;
            public string AnimationTriggerName;
            public GameObject abilityPrefab;
            private GameObject Instance;
            public float DelayForApplyGE;

            public float damageInterval;
            public float totalDamageTime;
            private float remainingDamageTime;
            private Coroutine damageCoroutine;

            public VFXCue preVFXCue;
            public AnimationCue animationCue;
            public VFXCue vfxCue;
            private GameplayCueDurationalSpec animationCueSpec;
            private List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

            public DotAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
            }

            /// <summary>
            /// What to do when the ability is cancelled.
            /// </summary>
            public override void CancelAbility() { }

            /// <summary>
            /// What happens when we activate the ability.
            /// </summary>
            protected override IEnumerator ActivateAbility()
            {
                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                //animatorComponent.SetTrigger(AnimationTriggerName);
                //var animationCueSpec = animationCue.CreateSpec(new GameplayCueParameters());
                animationCueSpec = animationCue.ApplyFrom(this, new GameplayCueParameters());
                animationCueSpec.OnAdd();
                var preVFXSpec = preVFXCue.ApplyFrom(this, new GameplayCueParameters());
                preVFXSpec.OnAdd();

                yield return new WaitForSeconds(DelayForApplyGE);

                preVFXSpec.OnRemove();

                // primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as DotAbilitySO).GameplayEffect);

                //var spawnPoint = Owner.GetComponent<CastPointComponent>()._castPoint;

                //Instance = GameObject.Instantiate(abilityPrefab);
                //Instance.transform.parent = spawnPoint;
                //Instance.name = "Dot_Ability";
                //Instance.transform.position = spawnPoint.position;
                //Instance.transform.rotation = spawnPoint.rotation;

                var vfxSpec = vfxCue.ApplyFrom(this, new GameplayCueParameters());
                vfxSpec.OnAdd();
                Instance = (vfxSpec as VFXCueSpec).vfxInstance;

                var CollisionComponent = Instance.GetComponent<AbilityCollision>();
                CollisionComponent.Source = this.Owner;

                CollisionComponent.OnHit += (AbilitySystemCharacter target) => {
                    //GameObject.Destroy(Instance);
                    Debug.Log($"DotAbility hitAsc.name {target.name}");
                    //target.ApplyGameplayEffectSpecToSelf(effectSpec);
                    if (targets.Contains(target)) return;
                    targets.Add(target);
                    
                    this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                };
                CollisionComponent.OnExit += (target) =>
                {
                    Debug.Log($"DotAbility Exit Asc.name {target.name}");
                    targets.Remove(target);
                };


                remainingDamageTime = totalDamageTime;
                damageCoroutine = Owner.StartCoroutine(DamageOverTime(effectSpec));

                yield return new WaitForSeconds(totalDamageTime);

                

                //yield return new WaitForSeconds(3);

                Debug.Log($"DotAbility END!!!!!!!!!");
                // Clean Up
                Destroy(Instance);
                vfxSpec.OnRemove();
                foreach (var target in targets)
                {
                    target.RemoveGameplayEffect(effectSpec);
                }
                targets.Clear();

                yield return null;
            }

            private IEnumerator DamageOverTime(GameplayEffectSpec effectSpec)
            {
                while (remainingDamageTime > 0)
                {
                    foreach (var target in targets)
                    {
                        this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                    }
                    remainingDamageTime -= damageInterval;
                    yield return new WaitForSeconds(damageInterval);
                }
            }

            /// <summary>
            /// Checks to make sure Gameplay Tags checks are met. 
            /// Since the target is also the character activating the ability,
            /// we can just use Owner for all of them.
            /// </summary>
            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags)
                        && AscHasAllTags(Owner, this.Ability.AbilityTags.TargetTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.TargetTags.IgnoreTags);
            }

            /// <summary>
            /// Logic to execute before activating the ability
            /// </summary>
            protected override IEnumerator PreActivate()
            {
                // Apply animations
                animatorComponent = Owner.GetComponent<Animator>();
                //animatorComponent.SetTrigger("Ability1");

                yield return null;
            }

            public override void EndAbility()
            {
                base.EndAbility();
                //animatorComponent.ResetTrigger(AnimationTriggerName);
                animationCueSpec.OnRemove();

                if (damageCoroutine != null)
                {
                    Owner.StopCoroutine(damageCoroutine);
                    damageCoroutine = null;
                }
            }
        }
    }
}

