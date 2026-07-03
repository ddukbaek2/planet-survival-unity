using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private int score;
    private bool isGameOver;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        score = 0;
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public bool IsGameOver() {
        return isGameOver;
    }

    public int GetScore() {
        return score;
    }

    public void AddScore(int amount) {
        score += amount;
    }

    public void GameOver() {
        if (isGameOver) {
            return;
        }
        isGameOver = true;
        Time.timeScale = 0f;
    }

    public void Restart() {
        Time.timeScale = 1f;
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }
}
