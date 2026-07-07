using UnityEngine;

namespace IdleFarm.Data.Item {
    public abstract class ShopItemData : ItemData {
        [Header("Shop")]
        [Min(0)]
        public double price;
    }
}
