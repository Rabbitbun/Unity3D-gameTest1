using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Charging Ability")]
    public class ChargingAbilitySO : AbstractAbilityScriptableObject
    {
        
        // Gameplay Effect to apply
        public GameplayEffectScriptableObject GameplayEffect;

        [ReadOnly] private bool IsChargingAbility = true;

        public string AnimationTriggerName;

        public string AnimationBoolName;

        [SerializeField] private InputReader _inputReader = default;

        [ReadOnly] public bool IsCharging = false;

        public bool IsSpellChantAbility;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new ChargingAbilitySpec(this, owner);
            spec.Level = owner.Level;
            spec.AnimationTriggerName = this.AnimationTriggerName;
            spec.AnimationBoolName = this.AnimationBoolName;

            spec._inputReader = this._inputReader;

            spec.IsCharging = this.IsCharging;

            spec.IsSpellChantAbility = this.IsSpellChantAbility;
            //...

            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class ChargingAbilitySpec : AbstractAbilitySpec
        {
            private Animator animatorComponent;

            public string AnimationTriggerName;

            public string AnimationBoolName;

            public InputReader _inputReader = default;

            public bool IsCharging = false;

            public bool IsSpellChantAbility;

            private float TimeCounter = 0.0f;

            public ChargingAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {

            }

            /// 這邊應該跟MeleeCombo邏輯類似 迴圈每次減少時間 時間結束之前有接到輸入就重製時間

            /// <summary>
            /// What to do when the ability is cancelled
            /// </summary>
            public override void CancelAbility() 
            {
                
            }

            /// <summary>
            /// What happens when we activate the ability.
            /// </summary>
            protected override IEnumerator ActivateAbility()
            {
                if (AnimationTriggerName != null)
                    animatorComponent.SetTrigger(AnimationTriggerName);
                if (AnimationTriggerName != null)
                    animatorComponent.SetBool(AnimationBoolName, true);
                TimeCounter = 0.0f;

                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);

                var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as ChargingAbilitySO).GameplayEffect);
                while (IsCharging)
                {
                    if (!CheckGameplayTags()) yield break;

                    TimeCounter += Time.deltaTime;
                    if (TimeCounter >= 0.1f)
                    {
                        this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                        this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                        Owner.AttributeSystem.UpdateAttributeCurrentValues();
                        this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                        Owner.AttributeSystem.UpdateAttributeCurrentValues();

                        TimeCounter = 0.0f;
                    }

                    yield return null;
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

            private void OnUpdateChargeFlag(int value)
            {
                if (value > 0)
                    IsCharging = true;
                else
                    IsCharging = false;
            }


            /// <summary>
            /// Logic to execute before activating the ability.
            /// </summary>
            protected override IEnumerator PreActivate()
            {
                animatorComponent = Owner.GetComponent<Animator>();
                IsCharging = true;

                // 註冊事件 執行中數字>=1 取消時數字為-1
                if (IsSpellChantAbility)
                {
                    _inputReader.startedChanting += OnUpdateChargeFlag;
                    _inputReader.stoppedChanting += OnUpdateChargeFlag;
                }


                yield return null;
            }

            public override void EndAbility()
            {
                base.EndAbility();
                if (animatorComponent != null)
                    animatorComponent.ResetTrigger(AnimationTriggerName);
                if (animatorComponent != null)
                    animatorComponent.SetBool(AnimationBoolName, false);

                // 取消註冊事件
                if (IsSpellChantAbility)
                {
                    _inputReader.startedChanting -= OnUpdateChargeFlag;
                    _inputReader.stoppedChanting -= OnUpdateChargeFlag;
                }
            }
        }
    }
}

