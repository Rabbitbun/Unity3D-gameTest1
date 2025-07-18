using System.Collections;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    /// <summary>
    /// Simple Ability that applies a Gameplay Effect to the activating character
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Simple Ability")]
    public class SimpleAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        /// <summary>
        /// Gameplay Effect to apply
        /// </summary>
        public GameplayEffectScriptableObject GameplayEffect;
        /// <summary>
        /// 預計要使用的Gameplay Cue
        /// </summary>
        public GameplayCue GameplayCue;
        /// <summary>
        /// 執行動畫後多久後要應用 Cue
        /// </summary>
        public float TimeToApplyCueAfterAnim;

        public AnimationClip AnimationClip;

        public string AnimationTriggerName;

        public bool EarlyEnd = false;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new SimpleAbilitySpec(this, owner);
            spec.Level = owner.Level;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class SimpleAbilitySpec : AbstractAbilitySpec
        {
            private GameplayCue gameplayCue;

            private float timeToApplyCueAfterAnim;

            private string animationTriggerName;

            private bool earlyEnd;

            public SimpleAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
                gameplayCue = (this.Ability as SimpleAbilityScriptableObject).GameplayCue;
                timeToApplyCueAfterAnim = (this.Ability as SimpleAbilityScriptableObject).TimeToApplyCueAfterAnim;
                animationTriggerName = (this.Ability as SimpleAbilityScriptableObject).AnimationTriggerName;
                earlyEnd = (this.Ability as SimpleAbilityScriptableObject).EarlyEnd;

                //animationClip = (this.Ability as SimpleAbilityScriptableObject).AnimationClip;
            }

            /// <summary>
            /// What to do when the ability is cancelled.  We don't care about there for this example.
            /// </summary>
            public override void CancelAbility() { }

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


                // Apply primary effect
                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as SimpleAbilityScriptableObject).GameplayEffect);
                this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

                
                Debug.Log("isActive: " + isActive.ToString());
                Debug.Log("gameplayCue.Update()!!");
                yield return gameplayCue.Update();
                Debug.Log("END gameplayCue.Update()!!");
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
                //gameplayCue.PrepareCue(this.Owner);
                // Apply animations
                Animator animatorComponent = Owner.GetComponent<Animator>();
                var animationEventSystemComponent = Owner.GetComponent<AnimationEventSystem>();
                animatorComponent.SetTrigger("Ability1");

                Debug.Log("Ability1 take times: " + animatorComponent.GetCurrentAnimatorStateInfo(0).length);

                yield return new WaitForSeconds(timeToApplyCueAfterAnim);

                // Apply Gameplay Cue
                gameplayCue.ApplyCue(this.Owner, null);

                yield return null;
            }

            public override void EndAbility()
            {
                gameplayCue.RemoveCue(Owner);
                base.EndAbility();
                
            }
        }
    }

}