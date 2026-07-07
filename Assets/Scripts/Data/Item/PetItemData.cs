using UnityEngine;

namespace IdleFarm.Data.Item {
    [UnityEngine.CreateAssetMenu(menuName = "Idle Farm/Shop/Pet Item")]
    public sealed class PetItemData : ShopItemData {
        [Header("Effect")]
        public PetEffectType effectType;
        [Range(0f, 1f)]
        public float bonusPercent;
    }
}