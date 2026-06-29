using IdleFarm.Core;
using System.Collections.Generic;
using UnityEngine;


namespace IdleFarm.UI.Popup {
    public class UpgradePopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;

        [SerializeField] private Transform content;
        [SerializeField] private UpgradeItemUI itemPrefab;

        private readonly List<UpgradeItemUI> items = new();


        private void OnEnable() {
            if (game != null) {
                game.StateChanged += Refresh;
            }

            CreateItems();
            Refresh();
        }

        private void CreateItems() {
            foreach (var item in items) {
                Destroy(item.gameObject);
            }

            items.Clear();

            foreach (var upgrade in game.Upgrades) {
                var ui = Instantiate(itemPrefab, content);
                Debug.Log($"Create UI : {upgrade.upgradeName}");
                ui.Initialize(game, upgrade);

                items.Add(ui);
            }
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
            foreach (var item in items) {
                item.Refresh();
            }
        }
    }
}
