using UnityEngine;

public class Enemy : MonoBehaviour {
    private int maxHealth = 2;
    private int currentHealth;
    private int attackPower = 1;
    private int defense;
    private float moveSpeed = 2.5f;

    private Transform targetTransform;
    private HealthBar healthBar;
    private EnemySpawner spawner;

    void Awake() {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null) {
            healthBar.SetRatio(1f);
        }
    }

    public void ApplyDefinition(EnemyDefinition definition) {
        maxHealth = definition.health;
        currentHealth = definition.health;
        attackPower = definition.attack;
        defense = definition.defense;
        moveSpeed = definition.moveSpeed;
        transform.localScale = new Vector3(definition.scale, definition.scale, definition.scale);
        if (healthBar != null) {
            healthBar.SetRatio(1f);
        }
    }

    public void SetTarget(Transform target) {
        targetTransform = target;
    }

    public void SetSpawner(EnemySpawner value) {
        spawner = value;
    }

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        if (targetTransform == null) {
            return;
        }
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = targetTransform.position;
        Vector3 moveDirection = targetPosition - currentPosition;
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;
        transform.position = currentPosition + moveDirection * moveSpeed * Time.deltaTime;
    }

    public void ApplyHit(int incomingAttack) {
        int damage = CombatFormula.ComputeDamage(incomingAttack, defense);
        currentHealth -= damage;
        CombatText.Show(transform.position, damage, Color.white);
        if (healthBar != null) {
            float ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        if (GameManager.Instance != null) {
            GameManager.Instance.AddKill();
        }
        Object.Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            playerHealth.ApplyHit(attackPower);
        }
        Object.Destroy(gameObject);
    }

    void OnDestroy() {
        if (spawner != null) {
            spawner.NotifyEnemyRemoved();
        }
    }
}
