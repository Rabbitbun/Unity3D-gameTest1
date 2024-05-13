using AbilitySystem;
using AbilitySystem.Authoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameplayTag.Authoring
{
    public static class TagManager
    {
        public static bool HasAnyTag(List<GameplayTagScriptableObject> tagsToCheck, List<GameplayTagScriptableObject> tagList)
        {
            foreach (var tagToCheck in tagsToCheck)
            {
                if (tagList.Contains(tagToCheck)) 
                    return true;
            }
            return false;
        }
        public static bool HasTag(GameplayTagScriptableObject tagToCheck, List<GameplayTagScriptableObject> tagList)
        {
            if (tagList.Contains(tagToCheck)) return true;
            return false;
        }

        public static bool CheckTagRequirements(AbilitySystemCharacter asc, List<GameplayTagScriptableObject> currentTags, List<GameplayTagScriptableObject> requiredTags, List<GameplayTagScriptableObject> forbiddenTags)
        {
            return true;
        }

        public static bool CheckApplicationTagRequirementsGE(AbilitySystemCharacter asc, GameplayEffectContainer ge, List<GameplayTagScriptableObject> currentTags)
        {
            // Debug.Log($"CheckTagRequirementsMetGE: {ge.name}");
            return CheckTagRequirements(asc, 
                currentTags, 
                ge.spec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags.ToList(), 
                ge.spec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags.ToList());
        }

        public static void UpdateTags(AbilitySystemCharacter source, AbilitySystemCharacter target, ref List<GameplayTagScriptableObject> currentTags, List<GameplayEffectContainer> appliedGameplayEffects, List<AbstractAbilitySpec> gameplayAbilities, Action<List<GameplayTagScriptableObject>, AbilitySystemCharacter, AbilitySystemCharacter, string> OnTagsChanged, string activationGUID)
        {
            //We could use HashSet instead of List here, if performance here is critical. HOWEVER, the tradeoff is that HashSets cannot contain multiple instances of the same tag. So you would not be able to stack tags.
            //There is some more room for optimization aswell, like removing these allocations
            var geTags = new List<GameplayTagScriptableObject>();
            foreach (var appliedGE in appliedGameplayEffects)
            {
                // Debug.Log($"    GE: {appliedGE.spec.GameplayEffect.name} Tags({appliedGE.spec.GameplayEffect.gameplayEffectTags.GrantedTags.ToList().Count}): [{string.Join(",", appliedGE.spec.GameplayEffect.gameplayEffectTags.GrantedTags.Select(x => x.name))}]");
                foreach (var tag in appliedGE.spec.GameplayEffect.gameplayEffectTags.GrantedTags)
                {
                    geTags.Add(tag);
                }
            }

            var gaTags = new List<GameplayTagScriptableObject>();
            foreach (var ability in gameplayAbilities)
            {
                // Debug.Log($"    GA Tags ({ability.Ability.AbilityTags.ActivationOwnedTags.ToList().Count}): [{string.Join(",", ability.Ability.AbilityTags.ActivationOwnedTags.Select(x => x.name))}]");
                if (ability.isActive)
                {
                    foreach (var tag in ability.Ability.AbilityTags.ActivationOwnedTags)
                    {
                        gaTags.Add(tag);
                    }
                }
            }

            // Debug.Log($"OnGameplayEffectsTagsChanged?: {!currentTags.SequenceEqual(newTags)}");
            var newTags = new List<GameplayTagScriptableObject>();
            newTags.AddRange(geTags);
            newTags.AddRange(gaTags);

            // Debug.Log($"   currentTags ({currentTags.Count}): [{string.Join(",", currentTags.Select(x => x.name))}] / newTags ({newTags.Count}): [{string.Join(",", newTags.Select(x => x.name))}]");
            if (!currentTags.SequenceEqual(newTags))
            { //Must run BEFORE calling OnTagsAdded/OnTagsRemoved. Because they will use currentTags to calculate their tag diff. If currentTags doesnt update, then we'll run into a infinite loop with TriggerAbilities where applied GEs still dont have their tags on currentTags, and will be retriggered because the appliedGEs tags will be put into newTags, even tough its a different GE being applied.
                currentTags = new List<GameplayTagScriptableObject>(newTags);
                OnTagsChanged?.Invoke(currentTags, source, target, activationGUID);
            }

        }
    }
}
