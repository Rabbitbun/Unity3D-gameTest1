using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Instant Ability")]
    public class InstantAbilitySO : AbstractAbilityScriptableObject
    {

        // Gameplay Effect to apply
        public GameplayEffectScriptableObject GameplayEffect;

        public string AnimationTriggerName;

        public string AnimationBoolName;

        //public float WaitForAnimation;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new InstantAbilitySpec(this, owner);
            spec.Level = owner.Level;
            spec.AnimationTriggerName = this.AnimationTriggerName;
            spec.AnimationBoolName = this.AnimationBoolName;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class InstantAbilitySpec : AbstractAbilitySpec
        {
            private Animator animatorComponent;

            public string AnimationTriggerName;

            public string AnimationBoolName;

            public InstantAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {

            }

            /// <summary>
            /// What to do when the ability is cancelled
            /// </summary>
            public override void CancelAbility() { }

            /// <summary>
            /// What happens when we activate the ability.
            /// </summary>
            protected override IEnumerator ActivateAbility()
            {
                if (AnimationTriggerName != null)
                    animatorComponent.SetTrigger(AnimationTriggerName);
                if (AnimationTriggerName != null)
                    animatorComponent.SetBool(AnimationBoolName, true);

                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                // Apply primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as InstantAbilitySO).GameplayEffect);
                this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

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
                animatorComponent = Owner.GetComponent<Animator>();


                yield return null;
            }

            public override void EndAbility()
            {
                base.EndAbility();
                if (animatorComponent != null)
                    animatorComponent.ResetTrigger(AnimationTriggerName);
                if (animatorComponent != null)
                    animatorComponent.SetBool(AnimationBoolName, false);
            }
        }
    }
}

