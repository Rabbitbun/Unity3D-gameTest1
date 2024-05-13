using System;
using GameplayTag.Authoring;
using UnityEngine;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayEffectTags
    {
        /// <summary>
        /// The tag that defines this gameplay effect
        /// </summary>
        [Tooltip("用於描述該 Gameplay Effect 的標籤。")]
        [SerializeField] public GameplayTagScriptableObject AssetTag;

        /// <summary>
        /// The tags this GE grants to the ability system character
        /// </summary>
        [Tooltip("包含了當這個 Gameplay Effect 被應用時，將授予給能力系統角色的標籤。")]
        [SerializeField] public GameplayTagScriptableObject[] GrantedTags;

        /// <summary>
        /// These tags determine if the GE is considered 'on' or 'off'
        /// </summary>
        [Tooltip("描述了用於判斷此 Gameplay Effect 是「開啟」還是「關閉」的標籤要求。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer OngoingTagRequirements;

        /// <summary>
        /// These tags must be present for this GE to be applied
        /// </summary>
        [Tooltip("描述了應用此 Gameplay Effect 的標籤要求。只有符合這些要求的情況下，此效果才會被應用。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer ApplicationTagRequirements;

        /// <summary>
        /// Tag requirements that will remove this GE
        /// </summary>
        [Tooltip("描述了移除此 Gameplay Effect 的標籤要求。當符合這些要求時，此效果將被移除。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer RemovalTagRequirements;

        /// <summary>
        /// Remove GE that match these tags
        /// </summary>
        [Tooltip("包含了與特定標籤相匹配的其他 Gameplay Effect，當此效果應用時，這些效果將被移除。")]
        [SerializeField] public GameplayTagScriptableObject[] RemoveGameplayEffectsWithTag;
    }

}
