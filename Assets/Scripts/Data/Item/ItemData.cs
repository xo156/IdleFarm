using UnityEngine;

namespace IdleFarm.Data.Item {
    public abstract class ItemData : ScriptableObject {
        [Header("Info")]
        public string id;
        public string displayName;
        public Sprite preview;
        [TextArea]
        public string description;
        public Sprite icon;
    }
}