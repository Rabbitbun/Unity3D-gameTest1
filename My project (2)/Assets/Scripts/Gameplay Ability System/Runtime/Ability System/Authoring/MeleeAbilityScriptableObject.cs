using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;

namespace AbilitySystem.Authoring
{
    /// <summary>
    /// Melee Ability that applies a Gameplay Effect to the activating character
    /// 可以有combo的動作片段
    /// </summary>
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Melee Ability")]
    public class MeleeAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        /// Gameplay Effect to apply
        public GameplayEffectScriptableObject GameplayEffect;

        /// 預計要使用的Gameplay Cue
        public GameplayCue GameplayCue;

        /// 執行動畫後多久後要應用 Cue
        //public float TimeToApplyCueAfterAnim;

        // public List<AnimationClip> animationClips;
        // public AnimationClip AnimationClip;

        public List<string> AnimationTriggerName;

        //public bool EarlyEnd;

        public bool HasCombo;

        public GameObject Collider;

        public List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

        public bool loggin = false;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new MeleeAbilitySpec(this, owner);
            spec.Level = owner.Level;

            spec.hasCombo = this.HasCombo;
            spec.colliderComponent = this.Collider;
            spec.targets = this.targets;
            spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
            spec.logging = this.loggin;

            //spec.comboTimes = this.ComboTimes;
            //spec.inputReader = this.inputReader;
            //inputReader.useAbility1Event += spec.testFoo;
            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class MeleeAbilitySpec : AbstractAbilitySpec
        {
            private GameplayCue gameplayCue;

            public CastPointComponent CastPointComponent;

            private GameplayEffectSpec effectSpec;

            private float timeToApplyCueAfterAnim;

            private List<string> animationTriggerName;

            private Animator animatorComponent;

            private bool earlyEnd;

            public bool logging;

            private float DurationRemaining;

            private int animationIndex = 0; // 需初始化
            
            public bool hasCombo = false; // 在外部決定

            //public int comboTimes = 0;  // 在外部決定 需初始化
            private int comboTimesLeft = 0;

            public bool shouldRestartingAbility = false; // 需初始化

            public GameObject colliderComponent;

            public List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

            public event Action<AbilitySystemCharacter> OnTargetCatched;

            public MeleeAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
                gameplayCue = (this.Ability as MeleeAbilityScriptableObject).GameplayCue;
                //timeToApplyCueAfterAnim = (this.Ability as MeleeAbilityScriptableObject).TimeToApplyCueAfterAnim;
                animationTriggerName = (this.Ability as MeleeAbilityScriptableObject).AnimationTriggerName;
                //earlyEnd = (this.Ability as MeleeAbilityScriptableObject).EarlyEnd;

            //animationClip = (this.Ability as SimpleAbilityScriptableObject).AnimationClip;
        }
            public override IEnumerator TryActivateAbility()
            {
                if (logging) Debug.Log("檢查是否可以使用Ability!!!!!!!!!!!!!!!!!");
                // 這邊只有檢查 tags、cost
                if (!CanActivateAbility()) yield break;
                // 檢查冷卻
                if (CheckCooldown().TimeRemaining > 0)
                {
                    if (logging) Debug.Log($"{this.Ability.name} is on Cooldown. Time Remaining: {CheckCooldown().TimeRemaining}");
                    Owner.OnGameplayAbilityFailedActivation?.Invoke(this, Ability.name, ActivationFailure.COOLDOWN);
                    yield break;
                }
                // 到這邊表示冷卻結束了, 接著判斷active以確認是否要做combo(hasCombo)
                if (isActive)
                {
                    if (logging) Debug.Log("檢查一些參數!!!!! hasCombo: " + hasCombo + " comboTimesLeft: " + comboTimesLeft);
                    // 若啟用中則可以重新啟動
                    Debug.Log("comboTimesLeft: " + comboTimesLeft);
                    if (hasCombo && comboTimesLeft > 1 && shouldRestartingAbility == false)
                    {
                        // comboTimesLeft
                        Debug.Log("設定重新啟動一次能力!!!!!!!");
                        shouldRestartingAbility = true;
                        comboTimesLeft--;
                    }
                    else yield break;
                }
                else
                {
                    if (logging) Debug.Log("正常使用Ability!!!!!!!!!!!!!!!!!");
                    isActive = true;
                    yield return PreActivate();
                    if (logging) Debug.Log("PreActivate 結束!!!!!!!!!!!!!!!!!");
                    yield return ActivateAbility();
                    if (logging) Debug.Log("ActivateAbility 結束!!!!!!!!!!!!!!!!!");
                }
                // 到這邊表示非啟動狀態且沒有冷卻，那就可以啟動了
                //EndAbility();
                if (isActive) yield return null;
                // 啟動完一次,接著確認是否要重新啟動能力
                if (shouldRestartingAbility && animationIndex < animationTriggerName.Count - 1)
                {
                    shouldRestartingAbility = false;
                    animationIndex++;
                    Debug.Log("重新啟動!!!!!!!!!!!!!!!!!!!!");
                    isActive = true;
                    yield return ActivateAbility();
                    if (logging) Debug.Log("ActivateAbility 結束!!!!!!!!!!!!!!!!!");
                }

                EndAbility();
            }

            public override bool CanActivateAbility()
            {
                if (!CheckGameplayTags())
                    return false;
                if (!CheckCost())
                {
                    Owner.OnGameplayAbilityFailedActivation?.Invoke(this, Ability.name, ActivationFailure.COST);
                    return false;
                }

                return true;
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
                Debug.Log("進入AA 檢查:" + "animationIndex: " + animationIndex);

                //List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>(null);


                // Apply cost and cooldown
                var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                // 應用動畫片段
                animatorComponent.SetTrigger(animationTriggerName[animationIndex]);
                if (logging) Debug.Log("MeleeAbility 需花費時間: " + animatorComponent.GetCurrentAnimatorStateInfo(0).length);
                //yield return new WaitForSeconds(animatorComponent.GetCurrentAnimatorStateInfo(0).length);

                

                // Apply primary effect
                this.effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as MeleeAbilityScriptableObject).GameplayEffect);
                DurationRemaining = this.effectSpec.DurationRemaining;

                var hitbox = GameObject.Instantiate(colliderComponent, CastPointComponent.GetSwordColliderPoint().transform);
                var hitboxCollider = hitbox.GetComponent<Collider>();
                //MeleeCollision meleeCollision = hitbox.GetComponent<MeleeCollision>();
                //meleeCollision.MeleeAbilitySpec = this;
                //meleeCollision.Source = this.Owner;

                // 在這邊偵測敵人
                while (DurationRemaining > 0)
                {
                    if (targets.Count == 0)
                    {
                        Collider[] colliders = Physics.OverlapBox(hitboxCollider.bounds.center, hitboxCollider.bounds.size / 2, hitboxCollider.transform.rotation);
                        foreach (Collider c in colliders)
                        {
                            AbilitySystemCharacter target = c.GetComponent<AbilitySystemCharacter>();
                            if (target != null && target != this.Owner && targets.Contains(target) == false)
                            {
                                // 應用GE
                                if (logging) Debug.Log("目標對象: " + target.name + "當前在列表內的對象: " + targets.Count);
                                target.ApplyGameplayEffectSpecToSelf(effectSpec);
                                targets.Add(target);
                            }
                        }
                    }

                    DurationRemaining -= Time.deltaTime;

                    if (!isActive) break;

                    yield return null;
                }
                GameObject.Destroy(hitbox);
                
                //this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

                //CheckConditionalGE(effectSpec);
                //yield return new WaitForSeconds(effectSpec.DurationRemaining);
                Debug.Log(" GE 應用完成 !!!!!!!!!!!!!!!!!");
                targets.Clear();

                //if (gameplayCue.IsActivated)
                //    Debug.Log(gameplayCue.IsActivated);
                //    yield return gameplayCue.Update();

                yield return null;
            }

            //public void CatchTarget(AbilitySystemCharacter target)
            //{
            //    if (target != null && targets.Contains(target) == false)
            //    {
            //        ApplyGEToTarget(target, this.effectSpec);
            //        targets.Add(target);
            //    }
            //}
            //private void ApplyGEToTarget(AbilitySystemCharacter target, GameplayEffectSpec effectSpec)
            //{
            //    target.ApplyGameplayEffectSpecToSelf(effectSpec);
            //}

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

                // 初始化參數
                animatorComponent = Owner.GetComponent<Animator>();
                animationIndex = 0;
                comboTimesLeft = animationTriggerName.Count;
                shouldRestartingAbility = false;
                //targets.Clear();
                //var animationEventSystemComponent = Owner.GetComponent<AnimationEventSystem>();

                //animatorComponent.SetTrigger(animationTriggerName[animationIndex]);

                //Debug.Log("MeleeAbility01 take times: " + animatorComponent.GetCurrentAnimatorStateInfo(0).length);

                //yield return new WaitForSeconds(timeToApplyCueAfterAnim);

                // Apply Gameplay Cue
                // gameplayCue.ApplyCue(this.Owner);

                yield return null;
            }

            public override void EndAbility()
            {
                gameplayCue.RemoveCue(Owner);
                // reset 所有 trigger
                foreach (var clip in animationTriggerName)
                {
                    animatorComponent.ResetTrigger(clip);
                }

                DurationRemaining = 0.0f;

                if (logging) Debug.Log("結束能力使用: 清除targets 列表成員!!!!");
                //targets.Clear();
                
                base.EndAbility();
            }
    }
}
}

