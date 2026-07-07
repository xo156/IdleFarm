using UnityEngine;

namespace IdleFarm.Data.Item {
    [UnityEngine.CreateAssetMenu(menuName = "Idle Farm/Shop/Consumable Item")]
    public sealed class ConsumableItemData : ShopItemData {
        [Header("Effect")]
        public string effectId;
        public double value;
    }
}