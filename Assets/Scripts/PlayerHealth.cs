using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int attackPower = 3;
    [SerializeField] private int defense = 1;

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

    public int GetAttackPower() {
        return attackPower;
    }

    public void AddMaxHealth(int amount) {
        maxHealth += amount;
        currentHealth += amount;
        if (healthBar != null) {
            float ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
    }

    public void AddAttackPower(int amount) {
        attackPower += amount;
    }

    public void AddDefense(int amount) {
        defense += amount;
    }

    public void ApplyHit(int incomingAttack) {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        int damage = CombatFormula.ComputeDamage(incomingAttack, defense);
        currentHealth -= damage;
        if (currentHealth < 0) {
            currentHealth = 0;
        }
        CombatText.Show(transform.position, damage, new Color(1f, 0.4f, 0.4f));
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
