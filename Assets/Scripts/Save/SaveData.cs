using System;
using System.Collections.Generic;

namespace IdleFarm.Save {
    // JSON 파일에 기록할 값만 모아둔 저장 전용 데이터 클래스
    [Serializable]
    public sealed class SaveData {
        public double gold;
        public double crops;

        public List<UpgradeSaveData> upgrades = new();
        public List<ItemSaveData> items = new();

        public string equippedPetId;
        public string equippedThemeId;

        public long lastSaveTimeTicks;

        public double prestigePoints;

        public double totalGoldEarned;
        public double totalCropsProduced;
        public double totalCropsHarvested;
        public int totalPrestigeCount;
    }
}
