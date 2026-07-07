using System;

namespace IdleFarm.Save {
    [Serializable]
    public sealed class ItemSaveData {
        public string itemId;
        public int quantity;
    }
}