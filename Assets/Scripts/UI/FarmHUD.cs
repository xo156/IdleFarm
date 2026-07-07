using IdleFarm.Constants;
using IdleFarm.Core;
using IdleFarm.Data.Item;
using IdleFarm.UI.Popup;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI {
    public sealed class FarmHUD : MonoBehaviour {
        [Header("Game")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;

        [Header("Texts")]
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text cropsText;
        [SerializeField] private TMP_Text productionText;
        [SerializeField] private TMP_Text cropNameText;

        [Header("Buttons")]
        [SerializeField] private Button harvestButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Button prestigeButton;
        [SerializeField] private Button shopButton;

        [Header("Items")]
        [SerializeField] private Image petImage;
        [SerializeField] private Image themeImage;

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(FarmHUD)} : Game reference is missing.");
            Debug.Assert(popupManager != null, $"{nameof(FarmHUD)} : PopupManager reference is missing.");
        }

        private void OnEnable() {
            Bind();
            Refresh();
        }

        private void OnDisable() {
            Unbind();
        }

        private void Bind() {
            if (game != null) {
                game.StateChanged -= Refresh;
                game.StateChanged += Refresh;
            }

            if (harvestButton != null) {
                harvestButton.onClick.RemoveListener(OnHarvestClicked);
                harvestButton.onClick.AddListener(OnHarvestClicked);
            }

            if (upgradeButton != null) {
                upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
                upgradeButton.onClick.AddListener(OnUpgradeClicked);
            }

            if (prestigeButton != null) {
                prestigeButton.onClick.RemoveListener(OnPrestigeClicked);
                prestigeButton.onClick.AddListener(OnPrestigeClicked);
            }

            if (shopButton != null) {
                shopButton.onClick.RemoveListener(OnShopClicked);
                shopButton.onClick.AddListener(OnShopClicked);
            }
        }

        private void Unbind() {
            if (game != null) {
                game.StateChanged -= Refresh;
            }

            if (harvestButton != null) {
                harvestButton.onClick.RemoveListener(OnHarvestClicked);
            }

            if (upgradeButton != null) {
                upgradeButton.onClick.RemoveListener(OnUpgradeClicked);
            }

            if (prestigeButton != null) {
                prestigeButton.onClick.RemoveListener(OnPrestigeClicked);
            }

            if (shopButton != null) {
                shopButton.onClick.RemoveListener(OnShopClicked);
            }
        }

        private void OnHarvestClicked() {
            game?.Harvest();
        }

        private void OnUpgradeClicked() {
            popupManager?.OpenUpgrade();
        }

        private void OnPrestigeClicked() {
            popupManager?.OpenPrestige();
        }

        private void OnShopClicked() {
            popupManager?.OpenShop();
        }

        private void Refresh() {
            if (game == null) {
                return;
            }

            if (goldText != null) {
                goldText.text = $"Gold : {NumberFormatter.Format(game.Gold)}";
            }

            if (cropsText != null) {
                cropsText.text = $"Crop : {NumberFormatter.Format(game.Crops)}";
            }

            if (productionText != null) {
                string interval = game.ProductionInterval.ToString("0.#", CultureInfo.InvariantCulture);
                productionText.text = $"Every {interval}s : +{NumberFormatter.Format(game.CropsPerProduction)} Crop";
            }

            if (harvestButton != null) {
                harvestButton.interactable = game.Crops > 0.0d;
            }

            RefreshCropName();
            RefreshPet();
            RefreshTheme();
        }

        private void RefreshCropName() {
            int level = game.GetUpgradeLevel(UpgradeIds.BetterSeeds);
            cropNameText.text = $"Lv.{level} {game.CropName}";
        }

        private void RefreshPet() {
            if (string.IsNullOrEmpty(game.EquippedPetId)) {
                petImage.enabled = false;
                return;
            }

            PetItemData pet = game.GetItem(game.EquippedPetId) as PetItemData;
            if (pet == null) {
                petImage.enabled = false;
                return;
            }

            petImage.enabled = true;
            petImage.sprite = pet.preview;
        }

        private void RefreshTheme() {
            if (string.IsNullOrEmpty(game.EquippedThemeId)) {
                return;
            }

            ThemeItemData theme = game.GetItem(game.EquippedThemeId) as ThemeItemData;
            if (theme == null) {
                return;
            }

            themeImage.sprite = theme.preview;
        }
    }
}