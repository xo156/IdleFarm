using System;
using System.Collections.Generic;
using IdleFarm.Save;
using IdleFarm.Data;
using UnityEngine;


namespace IdleFarm.Core {
    public sealed class IdleFarmGame : MonoBehaviour {
        [Header("Starting State")]
        [SerializeField] private double gold;
        [SerializeField] private double crops;

        [Header("Farm Tuning")]
        [SerializeField] private double baseCropsPerProduction = 1.0d;
        [SerializeField] private double productionInterval = 1.0d;
        [SerializeField] private double goldPerCrop = 1.0d;

        [Header("Upgrade")]
        [SerializeField] private List<UpgradeData> upgrades = new();
        private readonly Dictionary<UpgradeType, int> upgradeLevels = new();

        [Header("Offline")]
        [SerializeField] private double pendingOfflineCrops;
        public double PendingOfflineCrops => pendingOfflineCrops;

        private double productionTimer;

        // 변경 사항이 있으면 HUD가 화면을 새로 그리도록 알려주는 이벤트
        public event Action StateChanged;

        public double Gold => gold;
        public double Crops => crops;

        public double ProductionInterval => Math.Max(0.1d, productionInterval);

        public double CropsPerProduction =>
            baseCropsPerProduction +
            GetUpgradeLevel(UpgradeType.BetterSeeds) *
            GetUpgrade(UpgradeType.BetterSeeds).bonusPerLevel;

        public double CropsPerSecond => CropsPerProduction / ProductionInterval;

        public double GoldPerCrop =>
            goldPerCrop +
            GetUpgradeLevel(UpgradeType.CropQuality) *
            GetUpgrade(UpgradeType.CropQuality).bonusPerLevel;

        public int BetterSeedsLevel => GetUpgradeLevel(UpgradeType.BetterSeeds);

        public int CropQualityLevel => GetUpgradeLevel(UpgradeType.CropQuality);

        public double BetterSeedsProductionBonus => GetUpgrade(UpgradeType.BetterSeeds).bonusPerLevel;

        public double CropQualityPriceBonus => GetUpgrade(UpgradeType.CropQuality).bonusPerLevel;

        private void Awake() {
            LoadGame();
        }

        private void Update() {
            TickCropProduction(Time.deltaTime);

            HandleDebugInput();
        }

        private void HandleDebugInput() {
            if (Input.GetKeyDown(KeyCode.F12)) {
                ResetSave();
            }
        }

        private void OnApplicationQuit() {
            SaveGame();
        }

        private void OnApplicationPause(bool isPaused) {
            if (isPaused) {
                SaveGame();
            }
        }

        private void TickCropProduction(float deltaTime) {
            productionTimer += deltaTime;
            while (productionTimer >= ProductionInterval) {
                AddCrops(CropsPerProduction);
                productionTimer -= ProductionInterval;
            }
        }

        // 현재 보유한 작물을 모두 판매해서 Gold로 
        public void Harvest() {
            if (crops <= 0.0d) {
                return;
            }

            gold += crops * GoldPerCrop;
            crops = 0.0d;
            SaveGame();
            NotifyStateChanged();
        }

        public bool TryBuyUpgrade(UpgradeType type) {
            double cost = GetUpgradeCost(type);

            if (gold < cost) {
                // 코스트보다 돈이 없으면 업그레이드 못하니까
                return false;
            }

            gold -= cost;

            IncreaseUpgradeLevel(type);

            SaveGame();

            NotifyStateChanged();

            return true;
        }

        // 저장 파일을 지우고 현재 플레이 상태도 새 게임 상태로
        public void ResetSave() {
            Debug.Log("DEBUG RESET GAME");

            SaveManager.DeleteSave();
            ResetGameState();
            pendingOfflineCrops = 0.0d;
            productionTimer = 0.0d;
            SaveGame();
            NotifyStateChanged();
        }

        private void LoadGame() {
            if (!SaveManager.TryLoad(out var saveData)) {
                return;
            }

            gold = Math.Max(0.0d, saveData.gold);
            crops = Math.Max(0.0d, saveData.crops);
            upgradeLevels.Clear();
            upgradeLevels[UpgradeType.BetterSeeds] = Math.Max(0, saveData.betterSeedsLevel);
            upgradeLevels[UpgradeType.CropQuality] = Math.Max(0, saveData.cropQualityLevel);
            productionTimer = 0.0d;

            if (saveData.lastSaveTimeTicks > 0) {
                var lastSaveTime = new DateTime(saveData.lastSaveTimeTicks, DateTimeKind.Utc);
                double elapsedSeconds = (DateTime.UtcNow - lastSaveTime).TotalSeconds;
                elapsedSeconds = Math.Min(elapsedSeconds, 43200.0);
                pendingOfflineCrops = elapsedSeconds * CropsPerSecond;
            }
            else {
                pendingOfflineCrops = 0.0d;
            }
        }

        private void SaveGame() {
            SaveManager.Save(CreateSaveData());
        }

        private SaveData CreateSaveData() {
            return new SaveData {
                gold = gold,
                crops = crops,
                betterSeedsLevel = GetUpgradeLevel(UpgradeType.BetterSeeds),
                cropQualityLevel = GetUpgradeLevel(UpgradeType.CropQuality),
                lastSaveTimeTicks = DateTime.UtcNow.Ticks
            };
        }

        private void ResetGameState() {
            gold = 0.0d;
            crops = 0.0d;
            upgradeLevels.Clear();
            productionTimer = 0.0d;
            pendingOfflineCrops = 0.0d;
        }

        private double CalculateUpgradeCost(UpgradeType type) {
            UpgradeData data = GetUpgrade(type);
            if (data == null) {
                Debug.LogError($"UpgradeData missing in list: {type}");
                return 0;
            }

            int level = GetUpgradeLevel(type);
            return Math.Ceiling(data.baseCost * Math.Pow(data.costMultiplier, level));
        }

        public double GetUpgradeCost(UpgradeType type) {
            return CalculateUpgradeCost(type);
        }

        public bool CanBuyUpgrade(UpgradeType type) {
            return gold >= GetUpgradeCost(type);
        }

        public void AddCrops(double amount) {
            if (amount <= 0.0d) {
                return;
            }

            crops += amount;
            NotifyStateChanged();
        }

        public void ClaimOfflineReward() {
            if (pendingOfflineCrops <= 0.0d) {
                return;
            }

            AddCrops(pendingOfflineCrops);

            pendingOfflineCrops = 0.0d;

            SaveGame();
        }

        private void NotifyStateChanged() {
            StateChanged?.Invoke();
        }

        public IReadOnlyList<UpgradeData> Upgrades => upgrades;

        public UpgradeData GetUpgrade(UpgradeType type) {
            var upgrade = upgrades.Find(x => x.type == type);
            if (upgrade == null) {
                Debug.LogError($"UpgradeData not found : {type}");
            }

            return upgrade;
        }

        public int GetUpgradeLevel(UpgradeType type) {
            if (upgradeLevels.TryGetValue(type, out var level)) {
                return level;
            }
            return 0;
        }

        private void IncreaseUpgradeLevel(UpgradeType type) {
            upgradeLevels[type] = GetUpgradeLevel(type) + 1;
        }
    }
}
