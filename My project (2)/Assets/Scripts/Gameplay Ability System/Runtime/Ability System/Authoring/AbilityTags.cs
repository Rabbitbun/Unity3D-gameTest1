using UnityEngine;
using GameplayTag.Authoring;
using System;
using UnityEngine.Serialization;

namespace AbilitySystem.Authoring
{
    [Serializable]
    public struct AbilityTags
    {
        /// <summary>
        /// This tag describes the Gameplay Ability
        /// </summary>
        [SerializeField] public GameplayTagScriptableObject AssetTag;

        /// <summary>
        /// Active Gameplay Abilities (on the same character) that have these tags will be cancelled
        /// </summary>
        [Tooltip("包含了能夠取消當前能力的其他能力的標籤。如果同一個角色正在使用這些標籤中的能力，當前能力可能會被取消。")]
        [SerializeField] public GameplayTagScriptableObject[] CancelAbilitiesWithTags;

        /// <summary>
        /// Gameplay Abilities that have these tags will be blocked from activating on the same character
        /// </summary>
        [Tooltip("包含了能夠阻止當前能力在同一角色上啟動的其他能力的標籤。")]
        [SerializeField] public GameplayTagScriptableObject[] BlockAbilitiesWithTags;

        /// <summary>
        /// These tags are granted to the character while the ability is active
        /// </summary>
        [Tooltip("用於描述在能力啟動期間角色所擁有的標籤。當能力處於啟動狀態時，角色將具有這些標籤。")]
        [SerializeField] public GameplayTagScriptableObject[] ActivationOwnedTags;

        /// <summary>
        /// This ability can only be activated if the owner character has all of the Required tags
        /// and none of the Ignore tags.  Usually, the owner is the source as well.
        /// </summary>
        [Tooltip("描述了能力的擁有者（Owner）必須擁有的標籤。如果擁有者缺少所需的標籤或擁有不應該具有的標籤，則該能力可能無法啟動。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer OwnerTags;

        /// <summary>
        /// This ability can only be activated if the source character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [Tooltip("描述了能力的來源（Source）必須擁有的標籤。類似於 OwnerTags，但是針對來源角色。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer SourceTags;

        /// <summary>
        /// This ability can only be activated if the target character has all of the Required tags
        /// and none of the Ignore tags
        /// </summary>
        [Tooltip("描述了能力的目標（Target）必須擁有的標籤。類似於 OwnerTags，但是針對目標角色。")]
        [SerializeField] public GameplayTagRequireIgnoreContainer TargetTags;

    }

}