using IdleFarm.Core;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace IdleFarm.UI {
    public sealed class IdleFarmHud : MonoBehaviour {
        // Unity Inspector 또는 Bootstrap 코드에서 연결되는 UI 참조들
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text cropsText;
        [SerializeField] private TMP_Text productionText;
        [SerializeField] private TMP_Text cropPriceText;
        [SerializeField] private Button harvestButton;

        public void Initialize(
            IdleFarmGame game,
            TMP_Text goldText,
            TMP_Text cropsText,
            TMP_Text productionText,
            TMP_Text cropPriceText,
            Button harvestButton) {
            Unbind();

            this.game = game;
            this.goldText = goldText;
            this.cropsText = cropsText;
            this.productionText = productionText;
            this.cropPriceText = cropPriceText;
            this.harvestButton = harvestButton;

            Bind();
            Refresh();
        }

        private void Awake() {
            if (game == null) {
                game = FindFirstObjectByType<IdleFarmGame>();
            }
        }

        // OnEnable/OnDisable에서 이벤트와 버튼 클릭을 연결/해제
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
                harvestButton.onClick.RemoveListener(Harvest);
                harvestButton.onClick.AddListener(Harvest);
            }
        }

        private void Unbind() {
            if (game != null)  {
                game.StateChanged -= Refresh;
            }

            if (harvestButton != null) {
                harvestButton.onClick.RemoveListener(Harvest);
            }
        }

        private void Harvest() {
            game?.Harvest();
        }

        // 게임 숫자가 바뀔 때 화면 텍스트와 버튼 활성화 상태를 갱신
        private void Refresh() {
            if (game == null) {
                return;
            }

            if (goldText != null) {
                goldText.text = $"Gold: {NumberFormatter.Format(game.Gold)}";
            }

            if (cropsText != null) {
                cropsText.text = $"Crop: {NumberFormatter.Format(game.Crops)}";
            }

            if (productionText != null) {
                var interval = game.ProductionInterval.ToString("0.#", CultureInfo.InvariantCulture);
                productionText.text = $"Every {interval}s: +{NumberFormatter.Format(game.CropsPerProduction)} Crop";
            }

            if (cropPriceText != null) {
                cropPriceText.text = $"Crop Price: {NumberFormatter.Format(game.GoldPerCrop)} Gold";
            }

            if (harvestButton != null) {
                harvestButton.interactable = game.Crops > 0.0d;
            }
        }
    }
}
