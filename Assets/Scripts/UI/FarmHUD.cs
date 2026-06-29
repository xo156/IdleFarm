using UnityEngine;
using UnityEngine.UI;
using IdleFarm.Core;
using TMPro;


public class FarmHUD : MonoBehaviour {
    [Header("Top Bar")]
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text cropText;
    [SerializeField] private TMP_Text productionText;

    [Header("Main Area")]
    [SerializeField] private TMP_Text cropNameText;
    [SerializeField] private Slider growthSlider;
    [SerializeField] private Button harvestButton;

    [Header("Bottom Menu")]
    [SerializeField] private Button upgradeButton;

    private IdleFarmGame game;

    // Start is called before the first frame update
    void Start()
    {
        game = FindFirstObjectByType<IdleFarmGame>();

        harvestButton.onClick.AddListener(OnHarvest);

        RefreshUI();

        game.StateChanged += RefreshUI;
    }

    private void OnDestroy() {
        if (game != null)
            game.StateChanged -= RefreshUI;
    }

    private void OnHarvest() {
        game.Harvest();
    }

    private void RefreshUI() {
        goldText.text = $"Gold : {game.Gold:N0}";

        cropText.text = $"Crop : {game.Crops:N0}";

        productionText.text = $"{game.CropsPerSecond:F1}/s";

        cropNameText.text = $"Lv.{game.BetterSeedsLevel + 1} Wheat";

        growthSlider.value = Mathf.Clamp01((float)(game.Crops / 100.0));
    }
}
