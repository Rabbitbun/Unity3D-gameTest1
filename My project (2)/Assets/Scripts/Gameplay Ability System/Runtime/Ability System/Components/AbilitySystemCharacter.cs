using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTag.Authoring;
using UnityEngine;


namespace AbilitySystem
{
    public class AbilitySystemCharacter : MonoBehaviour
    {
        [SerializeField]
        protected AttributeSystemComponent _attributeSystem;
        public AttributeSystemComponent AttributeSystem { get { return _attributeSystem; } set { _attributeSystem = value; } }
        public List<AbstractAbilitySpec> GrantedAbilities = new List<AbstractAbilitySpec>();
        

        public Action<AbstractAbilitySpec, string> OnGameplayAbilityPreActivate, OnGameplayAbilityActivated, OnGameplayAbilityTryActivate, OnGameplayAbilityDeactivated;
        public Action<AbstractAbilitySpec, string, ActivationFailure> OnGameplayAbilityFailedActivation;
        public Action<AbstractAbilitySpec> OnGameplayAbilityGranted, OnGameplayAbilityUngranted;

        public List<GameplayEffectContainer> AppliedGameplayEffects = new List<GameplayEffectContainer>();
        public Action<GameplayEffectSpec> OnGameplayEffectApplied, OnGameplayEffectRemoved;
        public Action<List<GameplayEffectContainer>> OnGameplayEffectsChanged;

        public List<GameplayTagScriptableObject> Tags = new List<GameplayTagScriptableObject>();
        public Action<List<GameplayTagScriptableObject>, AbilitySystemCharacter, AbilitySystemCharacter, string> OnTagsChanged; // returns the new tags list when any change happens
        public Action<List<GameplayTagScriptableObject>, AbilitySystemCharacter, AbilitySystemCharacter, string> OnTagsInstant; // tags that were applied by an instant GE, they are a single instantaneous event.

        public float Level;

        private void Awake()
        {
            // EVENT HANDLES
            // Trigger GameplayEffects Handles
            OnGameplayEffectApplied += (geSpec) => { OnGameplayEffectsChanged?.Invoke(AppliedGameplayEffects); };
            OnGameplayEffectRemoved += (geSpec) => { OnGameplayEffectsChanged?.Invoke(AppliedGameplayEffects); };

            OnGameplayEffectApplied += UpdateTagsOnEffectChange;
            OnGameplayEffectRemoved += UpdateTagsOnEffectChange;

            //Process GA tags
            OnGameplayAbilityActivated += UpdateTagsOnGameplayAbilityActivate;
            OnGameplayAbilityDeactivated += UpdateTagsOnGameplayAbilityDeactivate;

            OnGameplayEffectApplied += TriggerOnTagsAdded;
        }

        private void Start()
        {
            var logging = false;
            //Loggers
            if (logging)
            {
                Debug.Log($"LOGGING {this.name}");
                OnTagsChanged += (newTags, src, tgt, applicationGUID) => Debug.Log($"[TAGS] OnTagsChanged! newTags: [{string.Join(", ", newTags.Select(x => x.name))}]");
                OnTagsInstant += (newTags, src, tgt, applicationGUID) => Debug.Log($"[TAGS] OnTagsInstant! tags: [{string.Join(", ", newTags.Select(x => x.name))}]");

                OnGameplayEffectsChanged += (ges) =>
                {
                    var geNames = new List<string>();
                    ges.ForEach(ge => geNames.Add(ge.spec.GameplayEffect.name));
                    // Debug.Log($"GameplayEffectsChanged, appliedGEs: {new JsonListWrapper<string>(geNames).ToJson()}");
                    Debug.Log($"GameplayEffectsChanged, appliedGEs: [{string.Join(", ", geNames)}]");
                };
                OnGameplayEffectApplied += (newGE) => Debug.Log($"OnGameplayEffectApplied ge: {newGE.GameplayEffect.name} ");
                OnGameplayEffectRemoved += (removedGE) => Debug.Log($"OnGameplayEffectRemoved ge: {removedGE.GameplayEffect.name}");
                OnGameplayAbilityFailedActivation += (ga, activationGUID, failureCause) => Debug.Log($"GA Failed Activation: {ga.Ability.name} {failureCause}");
            }
        }

        public void UpdateTagsOnEffectChange(GameplayEffectSpec ge)
        {
            TagManager.UpdateTags(ge.Source, ge.Target, ref Tags, AppliedGameplayEffects, GrantedAbilities, OnTagsChanged, ge.GameplayEffect.name);
            //TagManager.UpdateTags(ge.spec.Source, ge.spec.Target, ref Tags, AppliedGameplayEffects, GrantedAbilities, OnTagsChanged, ge.spec.GameplayEffect.name);
            //TagProcessor.UpdateTags(ge.source, ge.target, ref tags, appliedGameplayEffects, grantedGameplayAbilities, OnTagsChanged, ge.applicationGUID);
        }
        public void UpdateTagsOnGameplayAbilityActivate(AbstractAbilitySpec ga, string activationGUID)
        { //This needs to be a declared function, because we must remove this subscription for non owner client objects on multiplayer.
            TagManager.UpdateTags(ga.Owner, ga.Target, ref Tags, AppliedGameplayEffects, GrantedAbilities, OnTagsChanged, ga.Ability.name);
            //TagProcessor.UpdateTags(ga.source, ga.target, ref tags, appliedGameplayEffects, grantedGameplayAbilities, OnTagsChanged, activationGUID);
        }
        public void UpdateTagsOnGameplayAbilityDeactivate(AbstractAbilitySpec ga, string activationGUID)
        { //This needs to be a declared function, because we must remove this subscription for non owner client objects on multiplayer.
            TagManager.UpdateTags(ga.Owner, ga.Target, ref Tags, AppliedGameplayEffects, GrantedAbilities, OnTagsChanged, ga.Ability.name);
            //TagProcessor.UpdateTags(ga.source, ga.target, ref tags, appliedGameplayEffects, grantedGameplayAbilities, OnTagsChanged, activationGUID);
        }

        public void TriggerOnTagsAdded(GameplayEffectSpec appliedGE)
        {
            if (appliedGE.GameplayEffect.gameplayEffectTags.GrantedTags.ToList().Count == 0) return;
            if (appliedGE.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant)
                OnTagsInstant?.Invoke(appliedGE.GameplayEffect.gameplayEffectTags.GrantedTags.ToList(), appliedGE.Source, appliedGE.Target, appliedGE.GameplayEffect.name);

            //if (appliedGE.spec.GameplayEffect.gameplayEffectTags.GrantedTags.ToList().Count == 0) return;
            //if (appliedGE.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) 
            //    OnTagsInstant?.Invoke(appliedGE.spec.GameplayEffect.gameplayEffectTags.GrantedTags.ToList(), appliedGE.spec.Source, appliedGE.spec.Target, appliedGE.spec.GameplayEffect.name);
            
            //if (appliedGE.gameplayEffectTags.GrantedTags.Count == 0) return;
            //if (appliedGE.durationType == GameplayEffectDurationType.Instant) OnTagsInstant?.Invoke(appliedGE.gameplayEffectTags.GrantedTags, appliedGE.source, appliedGE.target, appliedGE.applicationGUID);

        }

        /// <summary>
        /// 將實例化後的 AbilitySpec 加到list, 隨後會 在 AbilityController 裡 TryActivateAbility
        /// </summary>
        /// <param name="spec"></param>
        public void GrantAbility(AbstractAbilitySpec spec)
        {
            this.GrantedAbilities.Add(spec);

            OnGameplayAbilityGranted?.Invoke(spec);
        }

        public void UseAbilitySpec(AbstractAbilitySpec spec)
        {
            StartCoroutine(spec.TryActivateAbility());
            OnGameplayAbilityTryActivate?.Invoke(spec, spec.Ability.name);
        }

        /// <summary>
        /// 移除有這個 tag 的 Ability(已啟用的), 呼叫UngrantAbility 取消 Ability, 觸發事件
        /// </summary>
        /// <param name="tag"></param>
        public void RemoveAbilitiesWithTag(GameplayTagScriptableObject tag)
        {
            for (var i = GrantedAbilities.Count - 1; i >= 0; i--)
            {
                if (GrantedAbilities[i].Ability.AbilityTags.AssetTag == tag)
                {
                    //GrantedAbilities.RemoveAt(i);
                    UngrantAbility(i);
                }
            }
        }

        public void UngrantAbility(int index)
        {
            // 引導至主要的 UngrantAbility 邏輯
            UngrantAbility(GrantedAbilities[index]);
        }

        /// <summary> 
        /// Ungrants an ability, this is the correct way to remove the ability so we can clean it's effects. 
        /// 
        /// </summary>
        public void UngrantAbility(AbstractAbilitySpec ga)
        {
            ga.CancelAbility();
            GrantedAbilities.Remove(ga);
            OnGameplayAbilityUngranted?.Invoke(ga);
        }

        // 方便取得全部的 Tags
        public List<GameplayTagScriptableObject> GetAllTags()
        {
            return Tags;
        }

        public bool ApplyGameplayEffectSpecToTarget(GameplayEffectSpec geSpec, AbilitySystemCharacter target)
        {
            if (geSpec == null || target == null) return true;
            bool tagRequirementsOK = CheckTagRequirementsMet(geSpec);

            if (tagRequirementsOK == false) return false;

            // 選擇 GE 持續時間
            switch (geSpec.GameplayEffect.gameplayEffect.DurationPolicy)
            {
                case EDurationPolicy.HasDuration:
                case EDurationPolicy.Infinite:
                    target.ApplyDurationalGameplayEffect(geSpec);
                    break;
                case EDurationPolicy.Instant:
                    target.ApplyInstantGameplayEffect(geSpec);
                    return true;
            }

            OnGameplayEffectApplied?.Invoke(geSpec);

            return true;
        }

        /// <summary>
        /// Applies the gameplay effect spec to self
        /// 應用 GE 到自己
        /// </summary>
        /// <param name="geSpec">GameplayEffectSpec to apply</param>
        public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
        {
            if (geSpec == null) return true;
            bool tagRequirementsOK = CheckTagRequirementsMet(geSpec);

            if (tagRequirementsOK == false) return false;

            // 選擇 GE 持續時間
            switch (geSpec.GameplayEffect.gameplayEffect.DurationPolicy)
            {
                case EDurationPolicy.HasDuration:
                case EDurationPolicy.Infinite:
                    ApplyDurationalGameplayEffect(geSpec);
                    break;
                case EDurationPolicy.Instant:
                    ApplyInstantGameplayEffect(geSpec);
                    return true;
            }

            // 開協程讓cue可以再延遲的時間發動

            //var gameplayCues = geSpec.GameplayEffect.gameplayCues;
            //for (int i = 0; i < gameplayCues.Count; i++)
            //{
            //    var cue = gameplayCues[i];
            //    cue.ApplyCue(geSpec.Source);
            //}

            OnGameplayEffectApplied?.Invoke(geSpec);

            return true;
        }

        /// <summary>
        /// 產生一個新的 GameplayEffectSpec
        /// </summary>
        /// <param name="GameplayEffect"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, float? level = 1f)
        {
            level = level ?? this.Level;
            return GameplayEffectSpec.CreateNew(
                GameplayEffect: GameplayEffect,
                Source: this,
                Level: level.GetValueOrDefault(1));
        }
        /// <summary>
        /// 檢查 GE 是否滿足 tag 條件, 若通過就可以得到Tag, 然後應用GE
        /// </summary>
        bool CheckTagRequirementsMet(GameplayEffectSpec geSpec)
        {
            /// Build temporary list of all gametags currently applied
            var appliedTags = new List<GameplayTagScriptableObject>();
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                appliedTags.AddRange(AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffectTags.GrantedTags);
            }

            // Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
            // 每個在 ApplicationTagRequirements.RequireTags 裡的 tag 都要在 character tags list 中才會 return true
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags.Length; i++)
            {
                if (!appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags[i]))
                {
                    return false;
                }
            }

            // No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
            // 只要有一個 ApplicationTagRequirements.IgnoreTags 裡的 tag 在 character tags list 中就會 return false
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags.Length; i++)
            {
                if (appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags[i]))
                {
                    return false;
                }
            }

            // 若都 OK 就 add appliedTags into AppliedTags
            //AppliedTags.AddRange(appliedTags);

            return true;
        }

        /// <summary>
        /// 應用 立即的GE, 應用 modifires, 然後調整 Attribute
        /// </summary>
        void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            // Apply all modifiers
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                magnitude += modifier.offset;
                var attribute = modifier.Attribute;
                this.AttributeSystem.GetAttributeValue(attribute, out var attributeValue);

                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeValue.BaseValue += magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeValue.BaseValue *= magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeValue.BaseValue = magnitude;
                        break;
                }
                this.AttributeSystem.SetAttributeBaseValue(attribute, attributeValue.BaseValue);
            }
            var gameplayCues = spec.GameplayEffect.gameplayCues;
            for (int i = 0; i < gameplayCues.Count; i++)
            {
                var cue = gameplayCues[i];
                var newCue = new GameplayCue(cue);
                newCue.ApplyCue(this, spec);
            }
        }
        /// <summary>
        /// 應用 有持續時間的GE,
        /// </summary>
        void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            // 建立一個用於存儲要應用的 Modifiers 的列表
            var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                magnitude += modifier.offset;
                var attributeModifier = new AttributeModifier();

                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeModifier.Add = magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeModifier.Multiply = magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeModifier.Override = magnitude;
                        break;
                }
                // 將計算完的修改器添加到要應用的修改器列表中
                modifiersToApply.Add(new GameplayEffectContainer.ModifierContainer() { Attribute = modifier.Attribute, Modifier = attributeModifier });
            }
            // 將持續時間型遊戲效果和相關的修改器添加到已應用的遊戲效果列表中
            AppliedGameplayEffects.Add(new GameplayEffectContainer() { spec = spec, modifiers = modifiersToApply.ToArray() });

            var gameplayCues = spec.GameplayEffect.gameplayCues;
            for (int i = 0; i < gameplayCues.Count; i++)
            {
                var cue = gameplayCues[i];
                var newCue = new GameplayCue(cue);
                newCue.ApplyCue(this, spec);
            }
        }

        public void RemoveGameplayEffect(GameplayEffectSpec spec)
        {
            //GameplayEffectContainer.RemoveGameplayEffectSpec(spec);
            for (int i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                var ge = AppliedGameplayEffects[i].spec;

                if (ge == spec)
                {
                    AppliedGameplayEffects.RemoveAt(i);
                    OnGameplayEffectRemoved?.Invoke(ge);
                    break;
                }
            }
        }

        void UpdateAttributeSystem()
        {
            // Set Current Value to Base Value (default position if there are no GE affecting that atribute)


            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var modifiers = this.AppliedGameplayEffects[i].modifiers;
                for (var m = 0; m < modifiers.Length; m++)
                {
                    var modifier = modifiers[m];
                    AttributeSystem.UpdateAttributeModifiers(modifier.Attribute, modifier.Modifier, out _);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        void TickGameplayEffects()
        {
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var ge = this.AppliedGameplayEffects[i].spec;

                // Can't tick instant GE
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) continue;

                // Update time remaining.  Stritly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
                // 剩餘更新時間。嚴格來說，它只對持續的 GE 有效，但計算無限的 GE 並無害處 
                ge.UpdateRemainingDuration(Time.deltaTime);

                // Tick the periodic component
                ge.TickPeriodic(Time.deltaTime, out var executePeriodicTick);
                if (executePeriodicTick)
                {
                    ApplyInstantGameplayEffect(ge);
                }
            }
        }

        void CleanGameplayEffects()
        {
            // 移除過期的 GE
            //this.AppliedGameplayEffects.RemoveAll(x => x.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration && x.spec.DurationRemaining <= 0);
            
            // 移除過期的GE 然後進行通知
            var removedGEs = this.AppliedGameplayEffects.Where(x => x.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration && x.spec.DurationRemaining <= 0).ToList();
            for (var i = 0; i < removedGEs.Count; i++)
            {
                Debug.Log($"Removed GE: {removedGEs[i].spec.GameplayEffect.name}");
                this.AppliedGameplayEffects.Remove(removedGEs[i]);
                OnGameplayEffectRemoved?.Invoke(removedGEs[i].spec);
            }
            
        }

        void Update()
        {
            // Reset all attributes to 0
            this.AttributeSystem.ResetAttributeModifiers();
            UpdateAttributeSystem();

            TickGameplayEffects();
            CleanGameplayEffects();
        }
    }
}


namespace AbilitySystem
{
    public class GameplayEffectContainer
    {
        public GameplayEffectSpec spec;
        public ModifierContainer[] modifiers;

        public class ModifierContainer
        {
            public AttributeScriptableObject Attribute;
            public AttributeModifier Modifier;
        }
    }
}