using System.Collections;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Projectile Ability")]
    public class ProjectileAbilitySO : AbstractAbilityScriptableObject
    {
        public GameplayEffectScriptableObject GameplayEffect; // Gameplay Effect to apply

        public string AnimationTriggerName;

        public float DelayForApplyGE;

        public GameObject ProjectilePrefab;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new ProjectileAbilitySpec(this, owner);
            spec.Level = owner.Level;
            spec.AnimationTriggerName = this.AnimationTriggerName;
            spec.projectilePrefab = this.ProjectilePrefab;
            spec.DelayForApplyGE = this.DelayForApplyGE;

            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class ProjectileAbilitySpec : AbstractAbilitySpec
        {
            private Animator animatorComponent;
            public string AnimationTriggerName;
            public GameObject projectilePrefab;
            private GameObject projectile;
            public float DelayForApplyGE;

            public ProjectileAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
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

                animatorComponent.SetTrigger(AnimationTriggerName);
                yield return new WaitForSeconds(DelayForApplyGE);

                // primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as ProjectileAbilitySO).GameplayEffect);

                var spawnPoint = Owner.GetComponent<CaculateAiming>()._castPoint;

                projectile = GameObject.Instantiate(projectilePrefab);
                projectile.name = "Projectile_Ability";
                projectile.transform.position = spawnPoint.position;
                projectile.transform.rotation = spawnPoint.rotation;

                //var rb = projectile.AddComponent<Rigidbody>();
                //rb.drag = 0;
                //rb.useGravity = false;
                //rb.AddForce(rb.transform.forward * 10f, ForceMode.Impulse);

                var projectileComponent = projectile.GetComponent<Projectile>();
                projectileComponent.source = this.Owner;
                projectileComponent.OnHit += (target) => {
                    GameObject.Destroy(projectile);
                    Debug.Log($"ProjectileAbility hitAsc.name {target.name}");
                    //target.ApplyGameplayEffectSpecToSelf(effectSpec);
                    //this.Target = target;
                    this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                };

                yield return null;
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
                animatorComponent.ResetTrigger(AnimationTriggerName);
            }
        }
    }
}

