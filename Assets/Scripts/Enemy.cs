using UnityEngine;

public class Enemy : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private int contactDamage = 1;

    private int currentHealth;
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

    public void TakeDamage(int amount) {
        currentHealth -= amount;
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
            playerHealth.TakeDamage(contactDamage);
        }
        Object.Destroy(gameObject);
    }

    void OnDestroy() {
        if (spawner != null) {
            spawner.NotifyEnemyRemoved();
        }
    }
}
