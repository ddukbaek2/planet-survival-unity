using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour {
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text fpsText;
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
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) {
            return;
        }
        if (levelText != null) {
            int currentLevel = gameManager.GetLevel();
            int currentExperience = gameManager.GetExperience();
            int requiredExperience = gameManager.GetRequiredExperience();
            levelText.text = "Lv. " + currentLevel + "\nEXP " + currentExperience + " / " + requiredExperience;
        }
        bool isGameOver = gameManager.IsGameOver();
        if (gameOverPanel != null && gameOverPanel.activeSelf != isGameOver) {
            gameOverPanel.SetActive(isGameOver);
        }
        if (isGameOver && resultText != null) {
            resultText.text = "도달 레벨: " + gameManager.GetLevel();
        }
    }

    public void OnRestartClicked() {
        if (GameManager.Instance != null) {
            GameManager.Instance.Restart();
        }
    }
}
