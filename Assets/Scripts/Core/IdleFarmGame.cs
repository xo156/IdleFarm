using IdleFarm.Constants;
using IdleFarm.Data;
using IdleFarm.Data.Item;
using IdleFarm.Save;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleFarm.Core {
    public sealed class IdleFarmGame : MonoBehaviour {
        #region Inspector
        [Header("Starting State")]
        [SerializeField] private double gold;
        [SerializeField] private double crops;

        [Header("Farm Tuning")]
        [SerializeField] private double baseCropsPerProduction = 1.0d;
        [SerializeField] private double productionInterval = 1.0d;
        [SerializeField] private double goldPerCrop = 1.0d;

        [Header("Crop")]
        [SerializeField] private List<string> cropNames = new() {
            "Wheat",
            "Carrot",
            "Corn",
            "Rice",
            "Blue Wheat",
            "Golden Wheat"
        };

        [Header("Upgrade")]
        [SerializeField] private List<UpgradeData> upgrades = new();

        [Header("Offline")]
        [SerializeField] private double pendingOfflineCrops;

        [Header("Prestige")] // 내실
        [SerializeField] private double prestigePoints;
        [SerializeField] private double prestigeBaseRequirement = 10000.0d;
        [SerializeField] private double prestigeRequirementMultiplier = 1.5d;

        [Header("Shop")]
        [SerializeField] private ShopDatabase shopDatabase;

        [Header("Player Statistics")]
        [SerializeField] private double totalGoldEarned;
        [SerializeField] private double totalCropsProduced;
        [SerializeField] private double totalCropsHarvested;
        [SerializeField] private int totalPrestigeCount;
        #endregion

        #region Runtime Field
        private Dictionary<string, int> upgradeLevels;
        private Dictionary<string, UpgradeData> upgradeDatabase;

        private readonly Dictionary<string, ItemData> itemLookup = new();
        private readonly Dictionary<string, int> ownedItems = new();

        private double productionTimer;

        private string equippedPetId;
        private string equippedThemeId;

        public event Action StateChanged;

        public double GoldPerCrop {
            get {
                double value = goldPerCrop;
                value += GetUpgradeBonus(UpgradeIds.CropQuality);
                value *= GetPetMultiplier(PetEffectType.GoldProduction);
                value *= GetPrestigeMultiplier();

                return value;
            }
        }

        public double CropsPerProduction {
            get {
                double value = baseCropsPerProduction;
                value += GetUpgradeBonus(UpgradeIds.BetterSeeds);
                value *= GetPetMultiplier(PetEffectType.CropProduction);
                value *= GetPrestigeMultiplier();

                return value;
            }
        }

        public double ProductionInterval {
            get {
                double value = productionInterval;
                value /= GetPetMultiplier(PetEffectType.ProductionSpeed);

                return Math.Max(0.1d, value);
            }
        }
        #endregion

        #region Property
        public double Gold => gold;
        public double Crops => crops;
        public double CropsPerSecond => CropsPerProduction / ProductionInterval;
        public IReadOnlyList<UpgradeData> Upgrades => upgrades;
        public double PendingOfflineCrops => pendingOfflineCrops;
        public double PrestigePoints => prestigePoints;
        public string EquippedPetId => equippedPetId;
        public string EquippedThemeId => equippedThemeId;
        public IReadOnlyDictionary<string, int> OwnedItems => ownedItems;
        public double TotalGoldEarned => totalGoldEarned;
        public double TotalCropsProduced => totalCropsProduced;
        public double TotalCropsHarvested => totalCropsHarvested;
        public int TotalPrestigeCount => totalPrestigeCount;
        public string CropName {
            get {
                int stage = GetUpgradeLevel(UpgradeIds.BetterSeeds) / 10;
                if (cropNames == null || cropNames.Count == 0) {
                    return "Unknown";
                }

                if (stage >= cropNames.Count) {
                    return cropNames[cropNames.Count - 1];
                }

                return cropNames[stage];
            }
        }
        #endregion

        #region Unity Messages
        private void Awake() {
            BuildUpgradeDatabase();
            BuildItemLookup();

            LoadGame();

            foreach (var item in ownedItems) {
                Debug.Log($"item: {item}");
            }
        }

        private void Update() {
            TickCropProduction(Time.deltaTime);
            HandleDebugInput();
        }

        private void OnApplicationQuit() => SaveGame();

        private void OnApplicationPause(bool isPaused) {
            if (isPaused) {
                SaveGame();
            }
        }
        #endregion

        #region Initialization
        private void BuildUpgradeDatabase() {
            upgradeDatabase = new Dictionary<string, UpgradeData>();
            upgradeLevels = new Dictionary<string, int>();

            foreach (var upgrade in upgrades) {
                if (upgrade == null || string.IsNullOrEmpty(upgrade.id)) {
                    Debug.LogError("Invalid UpgradeData");
                    continue;
                }

                upgradeDatabase[upgrade.id] = upgrade;
                upgradeLevels[upgrade.id] = 0;
            }
        }

        private void BuildItemLookup() {
            itemLookup.Clear();
            if (shopDatabase == null) {
                Debug.LogError("shopDatabase is missing.");
                return;
            }

            foreach (var item in shopDatabase.items) {
                if (item == null) {
                    continue;
                }

                if (itemLookup.ContainsKey(item.id)) {
                    Debug.LogError($"Duplicate Item ID : {item.id}");
                    continue;
                }

                itemLookup.Add(item.id, item);
            }
        }
        private void LoadGame() {
            if (!SaveManager.TryLoad(out var save)) {
                return;
            }

            // 음수값 안들어오도록
            gold = Math.Max(0.0d, save.gold);
            crops = Math.Max(0.0d, save.crops);

            totalGoldEarned = Math.Max(0.0d, save.totalGoldEarned);
            totalCropsProduced = Math.Max(0.0d, save.totalCropsProduced);
            totalCropsHarvested = Math.Max(0.0d, save.totalCropsHarvested);
            totalPrestigeCount = Math.Max(0, save.totalPrestigeCount);

            prestigePoints = Math.Max(0.0d, save.prestigePoints);

            equippedPetId = save.equippedPetId;
            equippedThemeId = save.equippedThemeId;

            // 업그레이드 불러오기
            foreach (var upgrade in save.upgrades) {
                if (upgradeLevels.ContainsKey(upgrade.upgradeId)) {
                    upgradeLevels[upgrade.upgradeId] = upgrade.level;
                }
            }

            // 아이템 불러오기
            ownedItems.Clear();
            foreach (var item in save.items) {
                if (item.quantity <= 0) {
                    continue;
                }

                ownedItems[item.itemId] = item.quantity;
            }

            // 오프라인 보상
            if (save.lastSaveTimeTicks > 0) {
                var last = new DateTime(save.lastSaveTimeTicks, DateTimeKind.Utc);
                double elapsed = (DateTime.UtcNow - last).TotalSeconds;
                elapsed = Math.Min(elapsed, 43200);
                pendingOfflineCrops = elapsed * CropsPerSecond * GetPetMultiplier(PetEffectType.OfflineReward);
            }

            // 현재 장착중인 아이템 불러오기
            if (!string.IsNullOrEmpty(equippedPetId) && GetItemCount(equippedPetId) <= 0) {
                equippedPetId = string.Empty;
            }

            if (!string.IsNullOrEmpty(equippedThemeId) && GetItemCount(equippedThemeId) <= 0) {
                equippedThemeId = string.Empty;
            }
        }
        #endregion

        #region Production
        private void TickCropProduction(float deltaTime) {
            productionTimer += deltaTime;

            while (productionTimer >= ProductionInterval) {
                AddCrops(CropsPerProduction);
                productionTimer -= ProductionInterval;
            }
        }

        public void AddGold(double amount) {
            if (amount <= 0) {
                return;
            }

            gold += amount;
            totalGoldEarned += amount;

            NotifyStateChanged();
        }

        public void AddCrops(double amount) {
            if (amount <= 0) {
                return;
            }

            crops += amount;
            totalCropsProduced += amount;
            NotifyStateChanged();
        }

        public void Harvest() {
            if (crops <= 0) {
                return;
            }

            double harvestedCrops = crops;
            double earnedGold = harvestedCrops * GoldPerCrop;
            gold += earnedGold;

            totalGoldEarned += earnedGold;
            totalCropsHarvested += harvestedCrops;
            crops = 0;

            SaveGame();
            NotifyStateChanged();
        }

        public void ClaimOfflineReward() {
            if (pendingOfflineCrops <= 0) {
                return;
            }

            AddCrops(pendingOfflineCrops);
            pendingOfflineCrops = 0;

            SaveGame();
            NotifyStateChanged();
        }
        #endregion

        #region Upgrade
        public bool TryBuyUpgrade(string id) {
            var data = GetUpgrade(id);
            if (data == null) {
                return false;
            }

            double cost = GetUpgradeCost(id);
            if (gold < cost) {
                return false;
            }

            gold -= cost;
            IncreaseUpgradeLevel(id);

            SaveGame();
            NotifyStateChanged();
            return true;
        }

        public int GetUpgradeLevel(string id) {
            return upgradeLevels.TryGetValue(id, out var level) ? level : 0;
        }

        private void IncreaseUpgradeLevel(string id) {
            upgradeLevels[id] = GetUpgradeLevel(id) + 1;
        }

        public UpgradeData GetUpgrade(string id) {
            return upgradeDatabase.TryGetValue(id, out var data) ? data : null;
        }

        public double GetUpgradeCost(string id) {
            var data = GetUpgrade(id);
            if (data == null) {
                return 0;
            }

            int level = GetUpgradeLevel(id);

            return Math.Ceiling(data.baseCost * Math.Pow(data.costMultiplier, level));
        }

        private double GetUpgradeBonus(string id) {
            UpgradeData data = GetUpgrade(id);
            if (data == null) {
                return 0;
            }

            return GetUpgradeLevel(id) * data.bonusPerLevel;
        }

        public bool CanBuyUpgrade(string id) {
            return gold >= GetUpgradeCost(id);
        }
        #endregion

        #region Item
        public ItemData GetItem(string itemId) {
            if (itemLookup.TryGetValue(itemId, out var item)) {
                return item;
            }

            Debug.LogError($"Item not found : {itemId}");
            return null;
        }

        public int GetItemCount(string itemId) {
            if (ownedItems.TryGetValue(itemId, out int count)) {
                return count;
            }

            return 0;
        }

        public bool TryBuyItem(string itemId) {
            ItemData item = GetItem(itemId);
            if (item is not ShopItemData shopItem) {
                return false;
            }

            if (gold < shopItem.price) {
                return false;
            }

            gold -= shopItem.price;

            switch (item) {
                case ConsumableItemData consumable:
                    UseConsumable(consumable);
                    break;

                case PetItemData pet:
                    AddItem(pet.id, 1);
                    break;

                case ThemeItemData Theme:
                    AddItem(Theme.id, 1);
                    break;
            }

            SaveGame();
            NotifyStateChanged();

            return true;
        }

        public void UseItem(string itemId) {
            ItemData item = GetItem(itemId);
            if (item == null) {
                return;
            }

            switch (item) {
                case PetItemData pet:
                    EquipPet(pet);
                    break;

                case ThemeItemData Theme:
                    EquipTheme(Theme);
                    break;
            }

            SaveGame();
            NotifyStateChanged();
        }

        private void UseConsumable(ConsumableItemData item) {
            switch (item.effectId) {
                case ShopEffects.AddCrop:
                    AddCrops(item.value);
                    break;

                case ShopEffects.AddGold:
                    AddGold(item.value);
                    break;

                default:
                    Debug.LogWarning($"Unknown Effect : {item.id}");
                    break;
            }
        }

        private void AddItem(string itemId, int amount) {
            if (amount <= 0)
                return;

            ownedItems[itemId] = GetItemCount(itemId) + amount;
        }
        #endregion

        #region Equip
        private void EquipPet(PetItemData pet) {
            if (pet == null) {
                return;
            }

            if (GetItemCount(pet.id) <= 0) {
                return;
            }

            equippedPetId = pet.id;
            Debug.Log($"Equip Pet : {pet.displayName}");
            SaveGame();
            NotifyStateChanged();
        }

        private void EquipTheme(ThemeItemData Theme) {
            if (Theme == null) {
                return;
            }

            if (GetItemCount(Theme.id) <= 0) {
                return;
            }

            equippedThemeId = Theme.id;
            Debug.Log($"Equip Theme : {Theme.displayName}");
            SaveGame();
            NotifyStateChanged();
        }

        public bool IsItemEquipped(ItemData item) {
            if (item == null) {
                return false;
            }

            switch (item) {
                case PetItemData pet:
                    return equippedPetId == pet.id;

                case ThemeItemData Theme:
                    return equippedThemeId == Theme.id;

                default:
                    return false;
            }
        }

        private double GetPetMultiplier(PetEffectType effect) {
            if (string.IsNullOrEmpty(equippedPetId))
                return 1.0;

            ItemData item = GetItem(equippedPetId);

            if (item is not PetItemData pet)
                return 1.0;

            if (pet.effectType != effect)
                return 1.0;

            return 1.0 + pet.bonusPercent;
        }
        #endregion

        #region Prestige
        public bool CanPrestige() {
            return totalGoldEarned >= GetNextPrestigeTarget();
        }

        public double GetNextPrestigeTarget() {
            return prestigeBaseRequirement * Math.Pow(prestigeRequirementMultiplier, prestigePoints);
        }

        public double GetPrestigeMultiplier() {
            return 1.0d + (prestigePoints * 0.05d);
        }

        public void DoPrestige() {
            if (!CanPrestige()) {
                return;
            }

            prestigePoints += 1.0d;
            totalPrestigeCount++;

            ResetSave();

            SaveGame();
            NotifyStateChanged();
        }
        #endregion

        #region Save
        private void SaveGame() {
            SaveManager.Save(CreateSaveData());
        }

        private SaveData CreateSaveData() {
            var save = new SaveData {
                gold = gold,
                crops = crops,
                lastSaveTimeTicks = DateTime.UtcNow.Ticks,
                totalGoldEarned = totalGoldEarned,
                totalCropsProduced = totalCropsProduced,
                totalCropsHarvested = totalCropsHarvested,
                totalPrestigeCount = totalPrestigeCount,
                prestigePoints = prestigePoints,
                equippedPetId = equippedPetId,
                equippedThemeId = equippedThemeId
            };

            // 업그레이드 목록
            foreach (var keyValue in upgradeLevels) {
                save.upgrades.Add(new UpgradeSaveData {
                    upgradeId = keyValue.Key,
                    level = keyValue.Value
                });
            }

            // 아이템 목록
            foreach (var keyValue in ownedItems) {
                save.items.Add(new ItemSaveData {
                    itemId = keyValue.Key,
                    quantity = keyValue.Value
                });
            }

            return save;
        }
        #endregion

        #region Rest
        private void ResetProgress() {
            gold = 0;
            crops = 0;
            upgradeLevels.Clear();
            pendingOfflineCrops = 0;
            productionTimer = 0;
        }

        private void ResetGame() {
            gold = 0;
            crops = 0;
            upgradeLevels.Clear();
            pendingOfflineCrops = 0;
            productionTimer = 0;
        }

        private void ResetSave() {
            SaveManager.DeleteSave();
            ResetGame();
            SaveGame();
            NotifyStateChanged();
        }

        public void ResetAllData() {
            SaveManager.DeleteSave();

            ResetProgress();

            prestigePoints = 0;

            totalGoldEarned = 0;
            totalCropsProduced = 0;
            totalCropsHarvested = 0;
            totalPrestigeCount = 0;

            ownedItems.Clear();
            equippedPetId = "";
            equippedThemeId = "";

            SaveGame();
            NotifyStateChanged();
        }
        #endregion

        #region Utility
        private void NotifyStateChanged() => StateChanged?.Invoke();

        private void HandleDebugInput() {
            if (Input.GetKeyDown(KeyCode.F12)) {
                // F12누르면 초기화
                ResetAllData();
            }
        }
        #endregion
    }
}
