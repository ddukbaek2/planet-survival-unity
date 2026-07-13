using UnityEngine;

public class PlayerHealth : MonoBehaviour {
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private int attackPower = 3;
    [SerializeField] private int defense = 1;

    private int currentHealth;
    private HealthBar healthBar;

    private void Awake() {
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
            var ratio = (float)currentHealth / maxHealth;
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
        var damage = CombatFormula.ComputeDamage(incomingAttack, defense);
        currentHealth -= damage;
        if (currentHealth < 0) {
            currentHealth = 0;
        }
        CombatText.Show(transform.position + new Vector3(0f, 0f, 0.8f), damage, new Color(1f, 0.15f, 0.15f));
        if (healthBar != null) {
            var ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
        if (currentHealth <= 0) {
            if (GameManager.Instance != null) {
                GameManager.Instance.GameOver();
            }
        }
    }
}
