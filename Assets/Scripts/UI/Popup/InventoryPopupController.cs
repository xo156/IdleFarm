using IdleFarm.Core;
using IdleFarm.Data.Item;
using IdleFarm.UI.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public class InventoryPopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private Transform content;
        [SerializeField] private InventoryItemUI itemPrefab;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button petButton;
        [SerializeField] private Button ThemeButton;
        [SerializeField] private Button etcButton;

        private InventoryCategory currentCategory = InventoryCategory.Pet;
        private readonly List<InventoryItemUI> itemUIs = new();

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(InventoryPopupController)} : Game reference is missing.");
            Debug.Assert(popupManager != null, $"{nameof(InventoryPopupController)} : PopupManager reference is missing.");
            Debug.Assert(closeButton != null, $"{nameof(InventoryPopupController)} : CloseButton reference is missing.");
            Debug.Assert(itemPrefab != null, $"{nameof(InventoryPopupController)} : itemPrefab reference is missing.");
            Debug.Assert(petButton != null, $"{nameof(InventoryPopupController)} : PetButton reference is missing.");
            Debug.Assert(ThemeButton != null, $"{nameof(InventoryPopupController)} : ThemeButton reference is missing.");
            Debug.Assert(etcButton != null, $"{nameof(InventoryPopupController)} : EtcButton reference is missing.");
            Debug.Assert(content != null, $"{nameof(InventoryPopupController)} : Content reference is missing.");

            closeButton.onClick.AddListener(OnClose);

            petButton.onClick.AddListener(OnPetTabClicked);
            ThemeButton.onClick.AddListener(OnThemeTabClicked);
            etcButton.onClick.AddListener(OnEtcTabClicked);

        }

        private void OnEnable() {
            if (game != null) {
                game.StateChanged += Refresh;
            }

            currentCategory = InventoryCategory.Pet; // 기본값은 pet

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

        private void OnClose() {
            popupManager.CloseCurrentPopup();
        }

        private void OnPetTabClicked() {
            if (currentCategory == InventoryCategory.Pet) {
                return;
            }

            currentCategory = InventoryCategory.Pet;
            Refresh();
        }

        private void OnThemeTabClicked() {
            if (currentCategory == InventoryCategory.Theme) {
                return;
            }

            currentCategory = InventoryCategory.Theme;
            Refresh();
        }

        private void OnEtcTabClicked() {
            if (currentCategory == InventoryCategory.Etc) {
                return;
            }

            currentCategory = InventoryCategory.Etc;
            Refresh();
        }

        private void Refresh() {
            foreach (var ui in itemUIs) {
                Destroy(ui.gameObject);
            }

            itemUIs.Clear();
            foreach (var pair in game.OwnedItems) {
                if (pair.Value <= 0) {
                    continue;
                }

                ItemData item = game.GetItem(pair.Key);
                if (item == null) {
                    continue;
                }

                if (!CanDisplay(item)) {
                    continue;
                }

                InventoryItemUI ui = Instantiate(itemPrefab, content);
                ui.Initialize(game, item, pair.Value);
                itemUIs.Add(ui);
            }
        }

        private bool CanDisplay(ItemData item) {
            switch (currentCategory) {
                case InventoryCategory.Pet:
                    return item is PetItemData;

                case InventoryCategory.Theme:
                    return item is ThemeItemData;

                case InventoryCategory.Etc:
                    return item is ConsumableItemData;

                default:
                    return false;
            }
        }
    }
}
