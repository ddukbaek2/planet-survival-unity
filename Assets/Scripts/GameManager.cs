using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private int level;
    private int experience;
    private int pendingLevelUps;
    private int money;
    private bool isGameOver;

    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        level = 1;
        experience = 0;
        pendingLevelUps = 0;
        money = 0;
        isGameOver = false;
        Time.timeScale = 1f;
    }

    public bool IsGameOver() {
        return isGameOver;
    }

    public int GetLevel() {
        return level;
    }

    public int GetExperience() {
        return experience;
    }

    public int GetRequiredExperience() {
        return level * 10;
    }

    public bool HasPendingLevelUp() {
        return pendingLevelUps > 0;
    }

    public void ConsumeLevelUp() {
        if (pendingLevelUps > 0) {
            pendingLevelUps -= 1;
        }
    }

    public void AddKill() {
        if (isGameOver) {
            return;
        }
        experience += 1;
        while (experience >= GetRequiredExperience()) {
            experience -= GetRequiredExperience();
            level += 1;
            pendingLevelUps += 1;
        }
    }

    public int GetMoney() {
        return money;
    }

    public void AddMoney(int amount) {
        if (isGameOver) {
            return;
        }
        money += amount;
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
