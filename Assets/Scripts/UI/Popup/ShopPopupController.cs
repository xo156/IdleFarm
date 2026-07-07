using IdleFarm.Core;
using IdleFarm.Data.Item;
using IdleFarm.UI.Shop;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public sealed class ShopPopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private ShopDatabase database;
        [SerializeField] private ShopItemUI itemPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Button closeButton;

        private readonly List<ShopItemUI> itemUIs = new();

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(ShopPopupController)} : Game reference is missing.");
            Debug.Assert(database != null, $"{nameof(PrestigePopupController)} : database reference is missing.");
            Debug.Assert(itemPrefab != null, $"{nameof(PrestigePopupController)} : itemPrefab reference is missing.");
            Debug.Assert(content != null, $"{nameof(PrestigePopupController)} : content reference is missing.");
            Debug.Assert(closeButton != null, $"{nameof(PrestigePopupController)} : closeButton reference is missing.");
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
            foreach (var item in itemUIs) {
                Destroy(item.gameObject);
            }

            itemUIs.Clear();

            foreach (var data in database.items) {
                ShopItemUI ui = Instantiate(itemPrefab, content);
                ui.Initialize(game, data);
                itemUIs.Add(ui);
            }
        }

        public void Show() {
            gameObject.SetActive(true);
        }

        public void Hide() {
            gameObject.SetActive(false);
        }

        private void Refresh() {
            foreach (var ui in itemUIs) {
                ui.Refresh();
            }
        }
    }
}