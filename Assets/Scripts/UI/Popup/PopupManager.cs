using IdleFarm.Core;
using UnityEngine;

namespace IdleFarm.UI.Popup {
    public class PopupManager : MonoBehaviour {
        [Header("Game")]
        [SerializeField] private IdleFarmGame game;

        [Header("Popups")]
        [SerializeField] private UpgradePopupController upgradePopup;
        [SerializeField] private OfflineRewardPopupController offlineRewardPopup;
        [SerializeField] private PrestigePopupController prestigePopup; 
        [SerializeField] private ShopPopupController shopPopup;
        [SerializeField] private InventoryPopupController inventoryPopup;
        [SerializeField] private CanvasGroup safeAreaCanvasGroup;

        private IPopup currentPopup; // 현재 열려있는 팝업
        private MonoBehaviour currentPopupBehaviour;

        public bool IsPopupOpen => currentPopup != null;

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(PopupManager)} : Game reference is missing.");
        }

        private void Start() {
            if (game.PendingOfflineCrops > 0) {
                OpenOfflineReward();
            }
        }

        private void Update() {
            if (!IsPopupOpen) {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                // 현재 열린 팝업을 ESC누르면 닫을 수 있도록 하기
                CloseCurrentPopup();
            }
        }

        private void OpenPopup(IPopup popup, MonoBehaviour popupBehaviour) {
            if (currentPopup == popup) {
                // 이미 열린거는 무시
                return;
            }

            if (currentPopup != null) {
                currentPopup.Hide();
            }

            currentPopup = popup;
            currentPopupBehaviour = popupBehaviour;
            safeAreaCanvasGroup.interactable = false;
            safeAreaCanvasGroup.blocksRaycasts = false;
            safeAreaCanvasGroup.alpha = 0.4f;

            currentPopup.Show();

            currentPopupBehaviour.transform.SetAsLastSibling();
        }

        public void OpenUpgrade() {
            OpenPopup(upgradePopup, upgradePopup);
        }

        public void OpenOfflineReward() {
            OpenPopup(offlineRewardPopup, offlineRewardPopup);
        }

        public void OpenPrestige() {
            OpenPopup(prestigePopup, prestigePopup);
        }

        public void OpenShop() {
            OpenPopup(shopPopup, shopPopup);
        }

        public void OpenInventory() {
            OpenPopup(inventoryPopup, inventoryPopup);
        }

        public void CloseCurrentPopup() {
            if (currentPopup == null) {
                return;
            }

            currentPopup.Hide();

            currentPopup = null;
            currentPopupBehaviour = null;

            safeAreaCanvasGroup.interactable = true;
            safeAreaCanvasGroup.blocksRaycasts = true;
            safeAreaCanvasGroup.alpha = 1.0f;
        }
    }
}