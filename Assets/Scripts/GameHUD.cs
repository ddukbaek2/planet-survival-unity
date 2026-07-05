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

    void Start() {
        smoothedFps = 60f;
        if (restartButton != null) {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }
    }

    void Update() {
        float deltaTime = Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
        smoothedFps = Mathf.Lerp(smoothedFps, 1f / deltaTime, 0.1f);
        if (fpsText != null) {
            fpsText.text = "FPS " + Mathf.RoundToInt(smoothedFps);
        }
        if (statsText != null) {
            int activeEnemies = Enemy.ActiveCount;
            int activeProjectiles = ProjectilePool.Instance != null ? ProjectilePool.Instance.GetActiveCount() : 0;
            int totalProjectiles = ProjectilePool.Instance != null ? ProjectilePool.Instance.GetTotalCount() : 0;
            statsText.text = "적 " + activeEnemies + "\n발사체 " + activeProjectiles + " / " + totalProjectiles;
        }

        if (playerHealth != null) {
            int currentHealth = playerHealth.GetCurrentHealth();
            int maxHealth = playerHealth.GetMaxHealth();
            float healthRatio = maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
            SetFill(healthFill, healthRatio);
            if (healthLabel != null) {
                healthLabel.text = "HP " + currentHealth + " / " + maxHealth;
            }
        }

        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) {
            return;
        }
        int level = gameManager.GetLevel();
        int experience = gameManager.GetExperience();
        int requiredExperience = gameManager.GetRequiredExperience();
        float experienceRatio = requiredExperience > 0 ? (float)experience / requiredExperience : 0f;
        SetFill(experienceFill, experienceRatio);
        if (levelLabel != null) {
            levelLabel.text = "Lv. " + level + "   " + experience + " / " + requiredExperience;
        }
        if (moneyLabel != null) {
            moneyLabel.text = "돈 " + gameManager.GetMoney();
        }

        bool isGameOver = gameManager.IsGameOver();
        if (gameOverPanel != null && gameOverPanel.activeSelf != isGameOver) {
            gameOverPanel.SetActive(isGameOver);
        }
    }

    void SetFill(RectTransform fill, float ratio) {
        if (fill == null) {
            return;
        }
        float clampedRatio = Mathf.Clamp01(ratio);
        Vector3 fillScale = fill.localScale;
        fillScale.x = clampedRatio;
        fill.localScale = fillScale;
    }

    public void OnRestartClicked() {
        if (GameManager.Instance != null) {
            GameManager.Instance.Restart();
        }
    }
}
