using UnityEngine;

public class PlayerShooter : MonoBehaviour {
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float targetingRange = 20f;

    private float fireTimer;

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) {
            return;
        }
        Enemy nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) {
            return;
        }
        fireTimer = fireInterval;
        Vector3 targetPosition = nearestEnemy.transform.position;
        FireAt(targetPosition);
    }

    Enemy FindNearestEnemy() {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        float nearestDistance = targetingRange;
        Vector3 currentPosition = transform.position;
        for (int index = 0; index < enemies.Length; index++) {
            Enemy candidateEnemy = enemies[index];
            Vector3 candidatePosition = candidateEnemy.transform.position;
            float distance = Vector3.Distance(currentPosition, candidatePosition);
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestEnemy = candidateEnemy;
            }
        }
        return nearestEnemy;
    }

    void FireAt(Vector3 targetPosition) {
        Vector3 spawnPosition = transform.position;
        Vector3 fireDirection = targetPosition - spawnPosition;
        fireDirection.y = 0f;
        fireDirection = fireDirection.normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(fireDirection);
        GameObject projectileObject = Object.Instantiate(projectilePrefab, spawnPosition, spawnRotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null) {
            projectile.SetDirection(fireDirection);
        }
    }
}
