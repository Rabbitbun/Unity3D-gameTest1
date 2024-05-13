using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using Unity.VisualScripting;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Melee Combo Ability")]
    public class MeleeComboAbilitySO : AbstractAbilityScriptableObject
    {
        /// Gameplay Effect to apply
        public GameplayEffectScriptableObject GameplayEffect;

        /// 預計要使用的Gameplay Cue
        public GameplayCue GameplayCue;

        public List<string> AnimationTriggerName;

        public bool HasCombo;

        public GameObject Collider;

        public List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

        //public List<GameplayCue> GameplayCueList = new List<GameplayCue>();
        
        public bool Cue_Queue_mod = false; // 一個一個應用
        public bool Cue_All_mod = false; // 全部一起應用

        public bool loggin = false;

        /// <summary>
        /// Creates the Ability Spec, which is instantiated for each character.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new MeleeComboAbilitySpec(this, owner);
            spec.Level = owner.Level;

            spec.hasCombo = this.HasCombo;
            spec.colliderComponent = this.Collider;
            spec.targets = this.targets;
            spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
            spec.logging = this.loggin;
            //spec.GameplayCueList = this.GameplayCueList;
            spec.Cue_Queue_mod = this.Cue_Queue_mod;
            spec.Cue_All_mod = this.Cue_All_mod;

            return spec;
        }

        /// <summary>
        /// The Ability Spec is the instantiation of the ability.  Since the Ability Spec
        /// is instantiated for each character, we can store stateful data here.
        /// 能力規格是能力的實例化。由於能力規格是為每個角色實例化的，因此我們可以在此處儲存狀態資料。
        /// </summary>
        public class MeleeComboAbilitySpec : AbstractAbilitySpec
        {
            private GameplayCue gameplayCue;

            public CastPointComponent CastPointComponent;

            private GameplayEffectSpec effectSpec;

            private List<string> animationTriggerName;

            private Animator animatorComponent;

            public bool logging;

            private float DurationRemaining;

            private int animationIndex = 0; // 需初始化

            public bool hasCombo = false; // 在外部決定

            //public int comboTimes = 0;  // 在外部決定 需初始化
            private int comboTimesLeft = 0;

            public bool shouldRestartingAbility = false; // 需初始化

            //public List<GameplayCue> GameplayCueList = new List<GameplayCue>();

            public bool Cue_Queue_mod = false; // 一個一個應用
            public bool Cue_All_mod = false; // 全部一起應用

            public GameObject colliderComponent;

            public List<AbilitySystemCharacter> targets = new List<AbilitySystemCharacter>();

            public event Action<AbilitySystemCharacter> OnTargetCatched;

            public MeleeComboAbilitySpec(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
                gameplayCue = (this.Ability as MeleeComboAbilitySO).GameplayCue;
                animationTriggerName = (this.Ability as MeleeComboAbilitySO).AnimationTriggerName;
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
                    //Owner.OnGameplayAbilityFailedActivation?.Invoke(this, Ability.name, ActivationFailure.COOLDOWN);
                    yield break;
                }
                // 到這邊表示冷卻結束了, 接著判斷active以確認是否要做combo(hasCombo)
                if (isActive)
                {
                    Debug.Log("檢查參數!! shouldRestartingAbility: " + shouldRestartingAbility + " comboTimesLeft: " + comboTimesLeft);
                    // 若啟用中則可以重新啟動
                    if (hasCombo && comboTimesLeft > 1 && shouldRestartingAbility == false)
                    {
                        // comboTimesLeft
                        Debug.Log("設定可以重新啟動一次能力!!!!!!!");
                        shouldRestartingAbility = true;
                        comboTimesLeft--;
                    }
                    yield break;
                }
                else
                {
                    if (logging) Debug.Log("正常使用Ability!!!!!!!!!!!!!!!!!");
                    isActive = true;
                    yield return PreActivate();
                    Debug.Log("PreActivate 結束!!!!!!!!!!!!!!!!!");
                    yield return ActivateAbility();
                    Debug.Log("ActivateAbility 結束!!!!!!!!!!!!!!!!!");
                }
                // 到這邊表示非啟動狀態且沒有冷卻，那就可以啟動了
                //EndAbility();
                //if (isActive) yield return null;

                // 啟動完一次,接著確認是否要重新啟動能力
                //if (shouldRestartingAbility && animationIndex < animationTriggerName.Count - 1)
                //{
                //    shouldRestartingAbility = false;
                //    animationIndex++;
                //    Debug.Log("重新啟動!!!!!!!!!!!!!!!!!!!!");
                //    isActive = true;
                //    yield return ActivateAbility();
                //    if (logging) Debug.Log("ActivateAbility 結束!!!!!!!!!!!!!!!!!");
                //}

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

            public void CheckConditionalGE(GameplayEffectSpec ge, bool queueMod, bool allMod)
            {
                // queueMod: 跟著動畫片段index一起
                if (queueMod)
                {
                    //if (animationIndex == 0) return;  // 跳過第0次
                    var GE = ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects[animationIndex];
                    if (AscHasAllTags(Owner, GE.RequiredSourceTags))
                    {
                        var geSpec = this.Owner.MakeOutgoingSpec(GE.GameplayEffect);
                        this.Owner.ApplyGameplayEffectSpecToSelf(geSpec);
                        Debug.Log("(QUEUE)應用額外 GE !!!!");
                    }
                    
                }
                if (allMod)
                {
                    for (int i = 0; i < ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects.Length; i++)
                    {
                        if (AscHasAllTags(Owner, ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects[i].RequiredSourceTags))
                        {
                            var geSpec = this.Owner.MakeOutgoingSpec(ge.GameplayEffect.gameplayEffect.ConditionalGameplayEffects[i].GameplayEffect);
                            this.Owner.ApplyGameplayEffectSpecToSelf(geSpec);
                            Debug.Log("(ALL)應用額外 GE !!!!");
                        }
                    }
                }

                
            }

            /// <summary>
            /// What happens when we activate the ability.
            /// </summary>
            protected override IEnumerator ActivateAbility()
            {
                do
                {
                    // 重新開始時的初始化變數
                    if (shouldRestartingAbility)
                    {
                        Debug.Log("重新執行一次能力!!!!");
                        shouldRestartingAbility = false;
                        animationIndex++;
                    }
                    
                    Debug.Log("進入AA 檢查:" + "animationIndex: " + animationIndex);

                    // Apply cost and cooldown
                    var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                    var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                    this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                    this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                    // 應用動畫片段
                    animatorComponent.SetTrigger(animationTriggerName[animationIndex]);

                    // primary effect
                    this.effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as MeleeComboAbilitySO).GameplayEffect);
                    DurationRemaining = this.effectSpec.DurationRemaining;

                    CheckConditionalGE(effectSpec, Cue_Queue_mod, Cue_All_mod);

                    if (animationIndex == 4) DurationRemaining += 0.3f;

                    // 碰撞體
                    var hitbox = GameObject.Instantiate(colliderComponent, CastPointComponent.GetSwordColliderPoint().transform);
                    var hitboxCollider = hitbox.GetComponent<Collider>();
                    //var hitboxCollider = colliderComponent.GetComponent<Collider>();

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
                        yield return new WaitForFixedUpdate();
                    }

                    Debug.Log(" GE 應用完成 !!!!!!!!!!!!!!!!!");
                    targets.Clear();
                    GameObject.Destroy(hitbox);

                } while (shouldRestartingAbility);
                



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
                animationIndex = 0;
                comboTimesLeft = animationTriggerName.Count;
                shouldRestartingAbility = false;



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

                // DurationRemaining = 0.0f;

                if (logging) Debug.Log("結束能力使用: 清除targets 列表成員!!!!");
                //targets.Clear();

                base.EndAbility();
            }
        }
    }

}
