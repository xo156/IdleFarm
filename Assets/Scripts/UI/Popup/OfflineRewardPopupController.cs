using IdleFarm.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public class OfflineRewardPopupController : MonoBehaviour, IPopup {
        [SerializeField] private IdleFarmGame game;

        [SerializeField] private TMP_Text rewardText;

        [SerializeField] private Button claimButton;

        [SerializeField] private GameObject popup;
        [SerializeField] private PopupManager popupManager;

        private void Awake() {
            claimButton.onClick.AddListener(OnClaimClicked);
        }

        private void Start() {
            Hide(); // ÀÏ´Ü ¼û±â±â
            if (game.PendingOfflineCrops > 0) {
                Show();
            }
        }

        public void Show() {
            rewardText.text = $"You earned\n{game.PendingOfflineCrops:N0} Crop";

            popup.SetActive(true);
        }

        public void Hide() {
            popup.SetActive(false);
        }

        private void OnClaimClicked() {
            game.ClaimOfflineReward();
            popupManager.CloseCurrentPopup();
        }
    }
}