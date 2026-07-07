using IdleFarm.Core;
using IdleFarm.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public class UpgradeItemUI : MonoBehaviour {
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private Button buyButton;

        private IdleFarmGame game;
        private string upgradeId;
        private UpgradeData data;

        public void Initialize(IdleFarmGame game, UpgradeData data) {
            this.game = game;
            this.data = data;
            this.upgradeId = data.id;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);

            Refresh();
        }

        public void Refresh() {
            if (game == null || data == null) {
                return;
            }

            int level = game.GetUpgradeLevel(upgradeId);
            double cost = game.GetUpgradeCost(upgradeId);

            infoText.text =
                $"{data.displayName}\n" +
                $"Lv {level}\n" +
                $"Cost : {cost:N0}\n" +
                $"+{data.bonusPerLevel}";

            buyButton.interactable = game.CanBuyUpgrade(upgradeId);
        }

        private void Buy() {
            game.TryBuyUpgrade(upgradeId);
        }
    }
}