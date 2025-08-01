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

        public bool ApplyPrefabTransform = false;

        public GameObject ProjectilePrefab;

        public int ProjectileNumber = 1;

        public AnimationCue AnimationCue;
        public VFXCue PreVFXCue;
        public GameObjectCue GameObjectCue;
        public PlaySoundCue SoundCue;

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
            spec.applyPrefabTransform = this.ApplyPrefabTransform;

            spec.projectileNumber = this.ProjectileNumber;

            spec.animationCue = this.AnimationCue;
            spec.preVfxCue = this.PreVFXCue;
            spec.gameObjectCue = this.GameObjectCue;
            spec.soundCue = this.SoundCue;

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
            private Projectile[] projectileComponents;
            public float DelayForApplyGE;
            public bool applyPrefabTransform;
            public int projectileNumber;

            public AnimationCue animationCue;
            public VFXCue preVfxCue;
            public GameObjectCue gameObjectCue;
            public PlaySoundCue soundCue;
            private GameplayCueDurationalSpec preVFXCueSpec;
            private GameplayCueDurationalSpec animationCueSpec;
            private GameplayCueDurationalSpec gameObjectCueSpec;
            private GameplayCueDurationalSpec soundCueSpec;

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

                if (animationCue != null)
                {
                    animationCueSpec = animationCue.ApplyFrom(this, new GameplayCueParameters());
                    animationCueSpec.OnAdd();
                }
                animatorComponent.SetTrigger(AnimationTriggerName); // else

                if (preVfxCue != null)
                {
                    preVFXCueSpec = preVfxCue.ApplyFrom(this, new GameplayCueParameters());
                    preVFXCueSpec.OnAdd();
                }
                if (gameObjectCue != null)
                {
                    gameObjectCueSpec = gameObjectCue.ApplyFrom(this, new GameplayCueParameters());
                    gameObjectCueSpec.OnAdd();
                }
                

                yield return new WaitForSeconds(DelayForApplyGE);

                if (preVfxCue != null)
                    preVFXCueSpec.OnRemove();
                if (gameObjectCue != null)
                    gameObjectCueSpec.OnRemove();
               
                if (soundCue != null)
                {
                    soundCueSpec = soundCue.ApplyFrom(this, new GameplayCueParameters());
                    soundCueSpec.OnAdd();
                }

                // primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as ProjectileAbilitySO).GameplayEffect);

                //var spawnPoint = Owner.GetComponent<CaculateAiming>()._castPoint;
                var spawnPoint = Owner.GetComponent<CastPointComponent>()._castPoint;

                float angleOffset = 20.0f; // 每個投射物的角度偏移量
                int halfNumber = projectileNumber / 2; // 中間點，奇數時會偏向左側

                for (int i = 0; i < projectileNumber; i++)
                {
                    float angle = (i - halfNumber) * angleOffset;
                    var rotation = Quaternion.Euler(0f, angle, 0f);

                    projectile = GameObject.Instantiate(projectilePrefab);
                    //projectile.transform.parent = spawnPoint;
                    projectile.name = "Projectile_Ability_" + i;
                    projectile.transform.position = spawnPoint.position;
                    projectile.transform.rotation = spawnPoint.rotation * rotation;
                    //projectile.transform.SetParent(null);
                    if (applyPrefabTransform)
                    {
                        projectile.transform.localPosition = projectilePrefab.transform.position;
                        projectile.transform.localRotation = projectilePrefab.transform.rotation;
                    }

                    projectileComponents[i] = projectile.GetComponent<Projectile>();
                    projectileComponents[i].source = this.Owner;
                    //projectileComponents[i].OnHit += (target) => {
                    //    GameObject.Destroy(projectile);
                    //    Debug.Log($"ProjectileAbility {i} hitAsc.name {target.name}");
                    //    //target.ApplyGameplayEffectSpecToSelf(effectSpec);
                    //    //this.Target = target;
                    //    this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                    //};
                }

                for (int i = 0; i < projectileNumber; i++)
                {
                    int index = i;
                    projectileComponents[index].OnHit += (target) =>
                    {
                        Debug.Log($"ProjectileAbility {index} hitAsc.name {target.name}");
                        this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                    };
                }
                //var rb = projectile.AddComponent<Rigidbody>();
                //rb.drag = 0;
                //rb.useGravity = false;
                //rb.AddForce(rb.transform.forward * 10f, ForceMode.Impulse);

                //var projectileComponent = projectile.GetComponent<Projectile>();
                //projectileComponent.source = this.Owner;
                //projectileComponent.OnHit += (target) => {
                //    GameObject.Destroy(projectile);
                //    Debug.Log($"ProjectileAbility hitAsc.name {target.name}");
                //    //target.ApplyGameplayEffectSpecToSelf(effectSpec);
                //    //this.Target = target;
                //    this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                //};

                yield return new WaitForSeconds(effectSpec.DurationRemaining);
                if (soundCue != null)
                {
                    soundCueSpec.OnRemove();
                }

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
                projectileComponents = new Projectile[projectileNumber];
                //animatorComponent.SetTrigger("Ability1");

                yield return null;
            }

            public override void EndAbility()
            {
                base.EndAbility();
                //animatorComponent.ResetTrigger(AnimationTriggerName);
                if (animationCueSpec != null)
                    animationCueSpec.OnRemove();
            }
        }
    }
}

