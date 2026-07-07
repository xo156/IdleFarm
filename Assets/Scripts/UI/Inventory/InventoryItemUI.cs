using TMPro;
using UnityEngine;
using UnityEngine.UI;
using IdleFarm.Core;
using IdleFarm.Data.Item;

namespace IdleFarm.UI.Inventory {
    public sealed class InventoryItemUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Button itemButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private Outline equippedOutline;

        private IdleFarmGame game;
        private ItemData item;
        private int quantity;

        public void Initialize(IdleFarmGame game, ItemData item, int quantity) {
            this.game = game;
            this.item = item;
            this.quantity = quantity;

            itemButton.onClick.RemoveListener(OnClicked);
            itemButton.onClick.AddListener(OnClicked);

            Refresh();
        }

        public void Refresh() {
            iconImage.sprite = item.icon;
            nameText.text = item.displayName;
            quantityText.text = $"x{quantity}";

            equippedOutline.enabled = game.IsItemEquipped(item);
        }

        private void OnClicked() {
            game.UseItem(item.id);
        }
    }
}