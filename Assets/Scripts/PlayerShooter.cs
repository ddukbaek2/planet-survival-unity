using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour {
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float fireInterval = 0.5f;
    [SerializeField] private float targetingRange = 20f;

    private float fireTimer;
    private readonly List<Enemy> enemiesInRange = new List<Enemy>();

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        fireTimer -= Time.deltaTime;
        if (fireTimer > 0f) {
            return;
        }
        CollectEnemiesInRange();
        if (enemiesInRange.Count == 0) {
            return;
        }
        fireTimer = fireInterval;
        int weaponCount = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
        for (int index = 0; index < weaponCount; index++) {
            Enemy target = enemiesInRange[index % enemiesInRange.Count];
            Vector3 targetPosition = target.transform.position;
            FireAt(targetPosition);
        }
    }

    void CollectEnemiesInRange() {
        enemiesInRange.Clear();
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Vector3 currentPosition = transform.position;
        for (int index = 0; index < enemies.Length; index++) {
            Enemy candidateEnemy = enemies[index];
            Vector3 candidatePosition = candidateEnemy.transform.position;
            float distance = Vector3.Distance(currentPosition, candidatePosition);
            if (distance <= targetingRange) {
                enemiesInRange.Add(candidateEnemy);
            }
        }
        Vector3 sortPosition = currentPosition;
        enemiesInRange.Sort((first, second) => {
            float firstDistance = (first.transform.position - sortPosition).sqrMagnitude;
            float secondDistance = (second.transform.position - sortPosition).sqrMagnitude;
            return firstDistance.CompareTo(secondDistance);
        });
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
