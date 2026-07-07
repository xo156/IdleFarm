using IdleFarm.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFarm.UI.Popup {
    public class PrestigePopupController : MonoBehaviour, IPopup {
        [Header("References")]
        [SerializeField] private IdleFarmGame game;
        [SerializeField] private PopupManager popupManager;
        [SerializeField] private TMP_Text currentPointText;
        [SerializeField] private TMP_Text gainPointText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text lifetimeGoldText;
        [SerializeField] private TMP_Text nextTargetText;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private void Awake() {
            Debug.Assert(game != null, $"{nameof(PrestigePopupController)} : Game reference is missing.");
            Debug.Assert(popupManager != null, $"{nameof(PrestigePopupController)} : PopupManager reference is missing.");
            Debug.Assert(confirmButton != null, $"{nameof(PrestigePopupController)} : confirmButton reference is missing.");
            Debug.Assert(cancelButton != null, $"{nameof(PrestigePopupController)} : cancelButton reference is missing.");

            confirmButton.onClick.AddListener(OnConfirm);
            cancelButton.onClick.AddListener(OnCancel);
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

            currentPointText.text =  $"Prestige Points : {game.PrestigePoints:N0}";
            gainPointText.text = "Reward : +1 Prestige Point";
            descriptionText.text = "Reset your progress and gain permanent bonuses.";
            lifetimeGoldText.text = $"Lifetime Gold : {NumberFormatter.Format(game.TotalGoldEarned)}";
            nextTargetText.text = $"Next Target : {NumberFormatter.Format(game.GetNextPrestigeTarget())} Gold";
            confirmButton.interactable = game.CanPrestige();
        }

        private void OnConfirm() {
            game.DoPrestige();
            popupManager.CloseCurrentPopup();
        }

        private void OnCancel() {
            popupManager.CloseCurrentPopup();
        }
    }
}