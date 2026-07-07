using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IdleFarm.Core;

namespace IdleFarm.UI.Popup {
    public class UpgradePopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private Button closeButton;
        [SerializeField] private Transform content;
        [SerializeField] private UpgradeItemUI itemPrefab;
        private readonly List<UpgradeItemUI> items = new();

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(UpgradePopupController)} : Game reference is missing.");
            Debug.Assert(popupManager != null, $"{nameof(UpgradePopupController)} : PopupManager reference is missing.");
            Debug.Assert(content != null, $"{nameof(UpgradePopupController)} : Content reference is missing.");
            Debug.Assert(itemPrefab != null, $"{nameof(UpgradePopupController)} : ItemPrefab reference is missing.");
            Debug.Assert(closeButton != null, $"{nameof(UpgradePopupController)} : CloseButton reference is missing.");

            closeButton.onClick.AddListener(OnClose);
        }
        private void OnEnable() {
            if (game != null) {
                game.StateChanged += Refresh;
            }

            CreateItems();
            Refresh();
        }

        private void OnDisable() {
            if (game != null) {
                game.StateChanged -= Refresh;
            }
        }

        private void CreateItems() {
            foreach (var item in items) {
                Destroy(item.gameObject);
            }

            items.Clear();

            foreach (var upgrade in game.Upgrades) {
                var ui = Instantiate(itemPrefab, content);
                ui.Initialize(game, upgrade);
                items.Add(ui);
            }
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Refresh() {
            foreach (var item in items) {
                item.Refresh();
            }
        }

        private void OnClose() {
            popupManager.CloseCurrentPopup();
        }
    }
}
