using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 5;

    private int currentHealth;
    private HealthBar healthBar;

    void Awake() {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null) {
            healthBar.SetRatio(1f);
        }
    }

    public int GetCurrentHealth() {
        return currentHealth;
    }

    public int GetMaxHealth() {
        return maxHealth;
    }

    public void TakeDamage(int amount) {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        currentHealth -= amount;
        if (currentHealth < 0) {
            currentHealth = 0;
        }
        if (healthBar != null) {
            float ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
        if (currentHealth <= 0) {
            if (GameManager.Instance != null) {
                GameManager.Instance.GameOver();
            }
        }
    }
}
