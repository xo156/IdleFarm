using UnityEngine;

namespace IdleFarm.Data {
    [CreateAssetMenu(menuName = "Idle Farm/Upgrade Data")]
    public class UpgradeData : ScriptableObject {
        public string id; // Save와 연결되는 Key 역할 
        public string displayName;

        public double baseCost;
        public double costMultiplier;
        public double bonusPerLevel;
    }
}