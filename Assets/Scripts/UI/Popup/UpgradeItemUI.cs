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
        private UpgradeData data;

        public void Initialize(IdleFarmGame game, UpgradeData data) {
            Debug.Log($"Initialize : {data.upgradeName}");
            this.game = game;
            this.data = data;

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Buy);

            Refresh();
        }

        public void Refresh() {
            if (game == null || data == null)
                return;

            int level = game.GetUpgradeLevel(data.type);

            infoText.text =
                $"{data.upgradeName}\n" +
                $"Lv {level}\n" +
                $"Cost : {game.GetUpgradeCost(data.type):N0}\n" +
                $"+{data.bonusPerLevel}";

            buyButton.interactable =
                game.CanBuyUpgrade(data.type);
        }

        private void Buy() {
            Debug.Log($"Buy : {data.upgradeName}");
            game.TryBuyUpgrade(data.type);
        }
    }
}