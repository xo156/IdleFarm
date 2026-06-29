using UnityEngine;

namespace IdleFarm.Data {
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "Idle Farm/Upgrade Data")]
    public class UpgradeData : ScriptableObject {
        [Header("Identity")]
        public UpgradeType type;

        [Header("Info")]
        public string upgradeName;

        [TextArea]
        public string description;

        [Header("Balance")]
        public double baseCost = 10;
        public double costMultiplier = 1.35;
        public double bonusPerLevel = 1;

        [Header("Limits")]
        public int maxLevel = 999;
    }
}