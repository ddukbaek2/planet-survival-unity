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

    private void Update() {
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
        var phase = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
        if (phase > lastElitePhase) {
            lastElitePhase = phase;
            if (phase >= 50) {
                if (!bossSpawned) {
                    bossSpawned = true;
                    var bossDefinition = EnemyTable.GetBoss();
                    SpawnSpecial(bossDefinition);
                    StageAnnouncer.Show("B O S S", bossDefinition.displayName, new Color(1f, 0.24f, 0.24f));
                }
            }
            else if (phase % 10 == 0) {
                var tier = phase / 10;
                var midBossDefinition = EnemyTable.GetMidBoss(tier);
                SpawnSpecial(midBossDefinition);
                StageAnnouncer.Show("MID BOSS", midBossDefinition.displayName, new Color(0.85f, 0.4f, 1f));
            }
            else {
                var eliteDefinition = EnemyTable.GetElite(phase);
                SpawnSpecial(eliteDefinition);
                StageAnnouncer.Show("STAGE " + phase, "네임드 몬스터 · " + eliteDefinition.displayName, new Color(1f, 0.64f, 0.18f));
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

    private void UpdateBossTraining() {
        if (GameManager.Instance != null && GameManager.Instance.HasPendingLevelUp()) {
            return;
        }
        if (!bossTrainingStarted) {
            bossTrainingStarted = true;
            var bossDefinition = EnemyTable.GetBoss();
            SpawnSpecial(bossDefinition);
            StageAnnouncer.Show("B O S S", bossDefinition.displayName, new Color(1f, 0.24f, 0.24f));
        }
    }

    private float GetCurrentSpawnInterval() {
        var difficultyProgress = Mathf.Clamp01(elapsedTime / difficultyRampSeconds);
        var currentInterval = Mathf.Lerp(initialSpawnInterval, minimumSpawnInterval, difficultyProgress);
        return currentInterval;
    }

    private int GetCurrentSpawnCount() {
        var difficultyProgress = Mathf.Clamp01(elapsedTime / difficultyRampSeconds);
        var currentCount = Mathf.Lerp(initialSpawnCount, maximumSpawnCount, difficultyProgress);
        return Mathf.RoundToInt(currentCount);
    }

    private void SpawnBurst(int phase) {
        var spawnCount = Mathf.Clamp(GetCurrentSpawnCount() + phase / 3, 1, 20);
        var aliveCap = Mathf.Min(maximumAliveEnemies, 60 + phase * 12);
        for (var index = 0; index < spawnCount; index++) {
            if (aliveEnemyCount >= aliveCap) {
                return;
            }
            SpawnEnemy();
        }
    }

    private void SpawnEnemy() {
        var randomAngle = Random.Range(0f, Mathf.PI * 2f);
        var playerPosition = playerTransform.position;
        var spawnOffset = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * spawnRadius;
        var spawnPosition = playerPosition + spawnOffset;
        spawnPosition.y = playerPosition.y;
        var enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null) {
            var definition = EnemyTable.PickForTime(elapsedTime);
            enemy.ApplyDefinition(definition);
            enemy.SetTarget(playerTransform);
            enemy.SetSpawner(this);
            var spawnLevel = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
            enemy.SetLevel(spawnLevel);
        }
        aliveEnemyCount++;
    }

    private void SpawnSpecial(EnemyDefinition definition) {
        var randomAngle = Random.Range(0f, Mathf.PI * 2f);
        var playerPosition = playerTransform.position;
        var spawnOffset = new Vector3(Mathf.Cos(randomAngle), 0f, Mathf.Sin(randomAngle)) * spawnRadius;
        var spawnPosition = playerPosition + spawnOffset;
        spawnPosition.y = playerPosition.y;
        var enemyObject = Object.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.ApplyDefinition(definition);
            enemy.SetTarget(playerTransform);
            enemy.SetSpawner(this);
            var spawnLevel = GameManager.Instance != null ? GameManager.Instance.GetLevel() : 1;
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
