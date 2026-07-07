using IdleFarm.Core;
using IdleFarm.Data.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Shop {
    public sealed class ShopItemUI : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Button itemButton;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text itemNameText;
        [SerializeField] private TMP_Text priceText;

        private IdleFarmGame game;
        private ShopItemData itemData;

        public void Initialize(IdleFarmGame game, ShopItemData itemData) {
            this.game = game;
            this.itemData = itemData;

            itemButton.onClick.RemoveListener(OnClickItem);
            itemButton.onClick.AddListener(OnClickItem);

            Refresh();
        }

        public void Refresh() {
            if (game == null || itemData == null) {
                return;
            }

            icon.sprite = itemData.icon;
            itemNameText.text = itemData.displayName;
            priceText.text = NumberFormatter.Format(itemData.price);
            itemButton.interactable = game.Gold >= itemData.price;
        }

        private void OnClickItem() {
            bool success = game.TryBuyItem(itemData.id);
            Debug.Log($"{itemData.displayName} Purchase : {success}");
        }

        private void OnDestroy() {
            if (itemButton != null) {
                itemButton.onClick.RemoveListener(OnClickItem);
            }
        }
    }
}