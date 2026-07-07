using IdleFarm.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public class OfflineRewardPopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Button claimButton;

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(OfflineRewardPopupController)} : Game reference is missing.");
            Debug.Assert(popupManager != null, $"{nameof(OfflineRewardPopupController)} : PopupManager reference is missing.");
            Debug.Assert(claimButton != null, $"{nameof(OfflineRewardPopupController)} : claimButton reference is missing.");

            claimButton.onClick.AddListener(OnClaimClicked);
        }

        private void OnEnable() {
            if (game != null) {
                game.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable() {
            if (game != null) {
                game.StateChanged -= Refresh;
            }
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Refresh() {
            if (game == null) {
                return;
            }

            rewardText.text = $"You earned\n{game.PendingOfflineCrops:N0} Crop";
        }

        private void OnClaimClicked() {
            game.ClaimOfflineReward();
            popupManager.CloseCurrentPopup();
        }
    }
}