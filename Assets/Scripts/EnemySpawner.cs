using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private float initialSpawnInterval = 0.5f;
    [SerializeField] private float minimumSpawnInterval = 0.12f;
    [SerializeField] private int initialSpawnCount = 1;
    [SerializeField] private int maximumSpawnCount = 5;
    [SerializeField] private int maximumAliveEnemies = 200;
    [SerializeField] private float difficultyRampSeconds = 45f;

    private float spawnTimer;
    private float elapsedTime;
    private int aliveEnemyCount;
    private int lastElitePhase;
    private bool bossSpawned;
    private bool bossTrainingStarted;

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        if (enemyPrefab == null || playerTransform == null) {
            return;
        }
        if (GameSelection.BossTraining) {
            UpdateBossTraining();
            return;
        }
        int phase = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
        if (phase > lastElitePhase) {
            lastElitePhase = phase;
            if (phase >= 50) {
                if (!bossSpawned) {
                    bossSpawned = true;
                    SpawnSpecial(EnemyTable.GetBoss());
                }
            }
            else {
                SpawnSpecial(EnemyTable.GetElite(phase));
            }
        }
        elapsedTime += Time.deltaTime;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer > 0f) {
            return;
        }
        spawnTimer = GetCurrentSpawnInterval();
        SpawnBurst(phase);
    }

    void UpdateBossTraining() {
        if (GameManager.Instance != null && GameManager.Instance.HasPendingLevelUp()) {
            return;
        }
        if (!bossTrainingStarted) {
            bossTrainingStarted = true;
            SpawnSpecial(EnemyTable.GetBoss());
        }
    }

    float GetCurrentSpawnInterval() {
        float difficultyProgress = Mathf.Clamp01(elapsedTime / difficultyRampSeconds);
        float currentInterval = Mathf.Lerp(initialSpawnInterval, minimumSpawnInterval, difficultyProgress);
        return currentInterval;
    }

    int GetCurrentSpawnCount() {
        float difficultyProgress = Mathf.Clamp01(elapsedTime / difficultyRampSeconds);
        float currentCount = Mathf.Lerp(initialSpawnCount, maximumSpawnCount, difficultyProgress);
        return Mathf.RoundToInt(currentCount);
    }

    void SpawnBurst(int phase) {
        int spawnCount = Mathf.Clamp(GetCurrentSpawnCount() + phase / 3, 1, 20);
        int aliveCap = Mathf.Min(maximumAliveEnemies, 60 + phase * 12);
        for (int index = 0; index < spawnCount; index++) {
            if (aliveEnemyCount >= aliveCap) {
                return;
            }
            SpawnEnemy();
        }
    }

    void SpawnEnemy() {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 playerPosition = playerTransform.position;
        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * spawnRadius;
        Vector3 spawnPosition = playerPosition + spawnOffset;
        spawnPosition.y = playerPosition.y;
        GameObject enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null) {
            EnemyDefinition definition = EnemyTable.PickForTime(elapsedTime);
            enemy.ApplyDefinition(definition);
            enemy.SetTarget(playerTransform);
            enemy.SetSpawner(this);
            int spawnLevel = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
            enemy.SetLevel(spawnLevel);
        }
        aliveEnemyCount++;
    }

    void SpawnSpecial(EnemyDefinition definition) {
        float randomAngle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 playerPosition = playerTransform.position;
        Vector3 spawnOffset = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * spawnRadius;
        Vector3 spawnPosition = playerPosition + spawnOffset;
        spawnPosition.y = playerPosition.y;
        GameObject enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.ApplyDefinition(definition);
            enemy.SetTarget(playerTransform);
            enemy.SetSpawner(this);
            int spawnLevel = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
            enemy.SetLevel(spawnLevel);
        }
        aliveEnemyCount++;
    }

    public void NotifyEnemyRemoved() {
        aliveEnemyCount--;
        if (aliveEnemyCount < 0) {
            aliveEnemyCount = 0;
        }
    }
}
