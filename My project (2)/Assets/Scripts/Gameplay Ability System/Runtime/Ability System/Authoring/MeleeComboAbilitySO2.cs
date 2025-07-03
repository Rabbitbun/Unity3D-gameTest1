using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Melee Combo Ability 2")]
    /// <summary>
    /// a melee combo ability in a single animation state
    /// </summary>
    public class MeleeComboAbilitySO2 : AbstractAbilityScriptableObject
    {
        /// Gameplay Effect to apply
        public List<GameplayEffectScriptableObject> GameplayEffects = new List<GameplayEffectScriptableObject>();

        public string AnimationTriggerName;

        public int AttackTimes = 1;

        public List<float> EachComboStartIn = new List<float>();

        public List<VFXCue> CollisionCues = new List<VFXCue>();

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new MeleeComboAbility2Spec(this, owner);
            spec.Level = owner.Level;

            spec.animationTriggerName = this.AnimationTriggerName;
            spec.attackTimes = this.AttackTimes;
            spec.eachComboStartIn = this.EachComboStartIn;
            spec.collisionCues = this.CollisionCues;

            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class MeleeComboAbility2Spec : AbstractAbilitySpec
        {
            private List<GameplayEffectSpec> effectSpecs = new List<GameplayEffectSpec>();

            private Animator animatorComponent;

            private List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

            public string animationTriggerName;

            public int attackTimes;

            public List<float> eachComboStartIn = new List<float>();

            private AbilityCollision[] collisions;

            public List<VFXCue> collisionCues = new List<VFXCue>();
            private List<GameplayCueDurationalSpec> collisionCueSpecs = new List<GameplayCueDurationalSpec>();

            public MeleeComboAbility2Spec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
                
            }

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

                foreach (var geSpec in (this.Ability as MeleeComboAbilitySO2).GameplayEffects)
                {
                    effectSpecs.Add(this.Owner.MakeOutgoingSpec(geSpec));
                }

                foreach (var cue in collisionCues)
                {
                    collisionCueSpecs.Add(cue.ApplyFrom(this, new GameplayCueParameters()));
                }

                // 應用動畫片段
                animatorComponent.SetTrigger(animationTriggerName);

                // combo attack logic
                for (int i = 0; i < attackTimes; i++)
                {
                    Debug.Log("Loop start: i: " + i);

                    int currentComboIndex = i; // Use a local variable to capture the current index
                    // 處理間隔時間, 假設 0.2, 1, 1.5
                    if (currentComboIndex == 0)
                    {
                        yield return new WaitForSeconds(eachComboStartIn[currentComboIndex]);
                        collisionCueSpecs[currentComboIndex].OnAdd();
                    }
                    else
                    {
                        yield return new WaitForSeconds(eachComboStartIn[currentComboIndex] - eachComboStartIn[currentComboIndex - 1]);
                        collisionCueSpecs[currentComboIndex - 1].OnRemove();
                        targets.Clear();
                        collisionCueSpecs[currentComboIndex].OnAdd();
                    }

                    GameObject instance = (collisionCueSpecs[currentComboIndex] as VFXCueSpec).vfxInstance;
                    collisions[currentComboIndex] = instance.GetComponent<AbilityCollision>();
                    collisions[currentComboIndex].Source = this.Owner;
                    collisions[currentComboIndex].OnHit += (AbilitySystemCharacter target) =>
                    {
                        if (targets.Contains(target)) return;
                        Debug.Log("i: " + i + ", index: " + currentComboIndex);
                        targets.Add(target);
                        this.Owner.ApplyGameplayEffectSpecToTarget(effectSpecs[currentComboIndex], target);
                    };
                }
                yield return new WaitForSeconds(1f);
                collisionCueSpecs[attackTimes - 1].OnRemove(); // last combo

                

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
            /// Logic to execute before activating the ability.
            /// </summary>
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
                targets.Clear();

            }
        }
    }

}
