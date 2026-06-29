using System;

namespace IdleFarm.Save {
    // JSON 파일에 기록할 값만 모아둔 저장 전용 데이터 클래스
    [Serializable]
    public sealed class SaveData {
        public double gold;
        public double crops;
        public int betterSeedsLevel;
        public int cropQualityLevel;
        public long lastSaveTimeTicks;
    }
}
