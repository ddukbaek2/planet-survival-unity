using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameHUD : MonoBehaviour {
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button restartButton;

    void Start() {
        if (restartButton != null) {
            restartButton.onClick.AddListener(OnRestartClicked);
        }
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(false);
        }
    }

    void Update() {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) {
            return;
        }
        if (scoreText != null) {
            scoreText.text = "점수: " + gameManager.GetScore();
        }
        bool isGameOver = gameManager.IsGameOver();
        if (gameOverPanel != null && gameOverPanel.activeSelf != isGameOver) {
            gameOverPanel.SetActive(isGameOver);
        }
        if (isGameOver && finalScoreText != null) {
            finalScoreText.text = "최종 점수: " + gameManager.GetScore();
        }
    }

    public void OnRestartClicked() {
        if (GameManager.Instance != null) {
            GameManager.Instance.Restart();
        }
    }
}
