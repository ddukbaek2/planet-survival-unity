using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour {
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private RectTransform experienceFill;
    [SerializeField] private TMP_Text levelLabel;
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private TMP_Text moneyLabel;
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;

    private float smoothedFps;

    private void Start() {
        smoothedFps = 60f;
        if (restartButton != null) {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update() {
        var deltaTime = Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        smoothedFps = Mathf.Lerp(smoothedFps, 1f / deltaTime, 0.1f);
        if (fpsText != null) {
            fpsText.text = "FPS " + Mathf.RoundToInt(smoothedFps);
        }
        if (statsText != null) {
            var activeEnemies = Enemy.ActiveCount;
            var activeProjectiles = ProjectilePool.Instance != null ? ProjectilePool.Instance.GetActiveCount() : 0;
            var totalProjectiles = ProjectilePool.Instance != null ? ProjectilePool.Instance.GetTotalCount() : 0;
            statsText.text = "적 " + activeEnemies + "\n발사체 " + activeProjectiles + " / " + totalProjectiles;
        }

        if (playerHealth != null) {
            var currentHealth = playerHealth.GetCurrentHealth();
            var maxHealth = playerHealth.GetMaxHealth();
            var healthRatio = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            SetFill(healthFill, healthRatio);
            if (healthLabel != null) {
                healthLabel.text = "HP " + currentHealth + " / " + maxHealth;
            }
        }

        var gameManager = GameManager.Instance;
        if (gameManager == null) {
            return;
        }
        var level = gameManager.GetLevel();
        var experience = gameManager.GetExperience();
        var requiredExperience = gameManager.GetRequiredExperience();
        var experienceRatio = requiredExperience > 0 ? (float)experience / requiredExperience : 0f;
        SetFill(experienceFill, experienceRatio);
        if (levelLabel != null) {
            levelLabel.text = "Lv. " + level + "   " + experience + " / " + requiredExperience;
        }
        if (moneyLabel != null) {
            moneyLabel.text = "돈 " + gameManager.GetMoney();
        }

        var isGameOver = gameManager.IsGameOver();
        if (gameOverPanel != null && gameOverPanel.activeSelf != isGameOver) {
            gameOverPanel.SetActive(isGameOver);
        }
    }

    private void SetFill(RectTransform fill, float ratio) {
        if (fill == null) {
            return;
        }
        var clampedRatio = Mathf.Clamp01(ratio);
        var fillScale = fill.localScale;
        fillScale.x = clampedRatio;
        fill.localScale = fillScale;
    }

    public void OnRestartClicked() {
        if (GameManager.Instance != null) {
            GameManager.Instance.Restart();
        }
    }
}
