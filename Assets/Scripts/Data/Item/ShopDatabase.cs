using System.Collections.Generic;
using UnityEngine;

namespace IdleFarm.Data.Item {
    [CreateAssetMenu(menuName = "Idle Farm/Shop/Shop Database")]
    public sealed class ShopDatabase : ScriptableObject {
        // 그냥 아이템 모아놓은 리스트
        public List<ShopItemData> items = new();
    }
}