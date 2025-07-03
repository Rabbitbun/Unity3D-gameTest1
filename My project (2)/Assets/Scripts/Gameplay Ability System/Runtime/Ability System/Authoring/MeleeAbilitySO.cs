using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    /// <summary>
    /// Melee Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Melee Ability")]
    public class MeleeAbilitySO : AbstractAbilityScriptableObject
    {
        /// Gameplay Effect to apply
        public GameplayEffectScriptableObject GameplayEffect;

        public string AnimationTriggerName;

        public float DelayForApplyGE;

        public GameObject Collider;

        public AnimationCue AnimationCue;
        public List<GameplayCueDurational> PreGECues = new List<GameplayCueDurational>();
        public List<VFXCue> CollisionCues = new List<VFXCue>();
        //public List<GameplayCueDurational> AfterGECues = new List<GameplayCueDurational>();

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new MeleeAbilitySpec(this, owner);
            spec.Level = owner.Level;

            spec.colliderComponent = this.Collider;
            spec.DelayForApplyGE = this.DelayForApplyGE;

            spec.animationCue = this.AnimationCue;
            spec.preGECues = this.PreGECues;
            //spec.afterGECues = this.AfterGECues;
            spec.collisionCues = this.CollisionCues;
            spec.animationTriggerName = this.AnimationTriggerName;
            
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class MeleeAbilitySpec : AbstractAbilitySpec
        {
            private GameplayEffectSpec effectSpec;

            private Animator animatorComponent;

            public string animationTriggerName;

            public float DelayForApplyGE;

            public GameObject colliderComponent;

            private AbilityCollision[] collisions;

            public List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

            public AnimationCue animationCue;
            private GameplayCueDurationalSpec animationCueSpec;
            public List<GameplayCueDurational> preGECues = new List<GameplayCueDurational>();
            public List<VFXCue> collisionCues = new List<VFXCue>();
            //public List<GameplayCueDurational> afterGECues = new List<GameplayCueDurational>();
            private List<GameplayCueDurationalSpec> preGECueSpecs = new List<GameplayCueDurationalSpec>();
            private List<GameplayCueDurationalSpec> collisionCueSpecs = new List<GameplayCueDurationalSpec>();
            //private List<GameplayCueDurationalSpec> afterGECueSpecs = new List<GameplayCueDurationalSpec>();

            public MeleeAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
              
            }   

            public void CheckConditionalGE(GameplayEffectSpec ge)
            {
                for (int i = 0; i < ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects.Length; i++)
                {
                    if(AscHasAllTags(Owner, ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects[i].RequiredSourceTags))
                    {
                        var geSpec = this.Owner.MakeOutgoingSpec(ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects[i].GameplayEffect);
                        this.Owner.ApplyGameplayEffectSpecToSelf(geSpec);
                    }
                }
            }

            /// <summary>
            /// What happens when we activate the ability.
            /// 
            /// In this example, we apply the cost and cooldown, and then we apply the main
            /// gameplay effect
            /// </summary>
            /// <returns></returns>
            protected override IEnumerator ActivateAbility()
            {
                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                // 動畫
                if (animationCue != null)
                {
                    animationCueSpec = animationCue.ApplyFrom(this, new GameplayCueParameters());
                    animationCueSpec.OnAdd();
                }
                animatorComponent.SetTrigger(animationTriggerName);
                // Cue
                foreach (var cue in preGECues)
                {
                    var spec = cue.ApplyFrom(this, new GameplayCueParameters());
                    preGECueSpecs.Add(spec);
                    spec.OnAdd();
                }

                yield return new WaitForSeconds(DelayForApplyGE);

                // GE
                this.effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as MeleeAbilitySO).GameplayEffect);

                for (int i = 0; i < collisionCues.Count; i++)
                {
                    var spec = collisionCues[i].ApplyFrom(this, new GameplayCueParameters());
                    collisionCueSpecs.Add(spec);
                    spec.OnAdd();
                    
                    GameObject instance = (spec as VFXCueSpec).vfxInstance;
                    collisions[i] = instance.GetComponent<AbilityCollision>();
                    collisions[i].Source = this.Owner;
                    collisions[i].OnHit += (AbilitySystemCharacter target) => 
                    {
                        if (targets.Contains(target)) return;
                        targets.Add(target);
                        this.Owner.ApplyGameplayEffectSpecToTarget(effectSpec, target);
                    };
                }

                foreach (var spec in preGECueSpecs)
                {
                    spec.OnRemove();
                }

                yield return new WaitForSeconds(0.7f);

                foreach (var spec in collisionCueSpecs)
                {
                    spec.OnRemove();
                }

                yield return null;
            }

            
            /// <summary>
            /// Checks to make sure Gameplay Tags checks are met. 
            /// 
            /// Since the target is also the character activating the ability,
            /// we can just use Owner for all of them.
            /// </summary>
            /// <returns></returns>
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
            /// Logic to execute before activating the ability.  We don't need to do anything here
            /// for this example.
            /// </summary>
            /// <returns></returns>

            protected override IEnumerator PreActivate()
            {
                // 初始化參數
                animatorComponent = Owner.GetComponent<Animator>();
                collisions = new AbilityCollision[collisionCues.Count];

                yield return null;
            }

            public override void EndAbility()
            {
                base.EndAbility();

                
            }
    }
}
}

