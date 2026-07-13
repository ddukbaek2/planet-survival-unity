using UnityEngine;

public class Enemy : MonoBehaviour {
    public static int ActiveCount;

    private int maxHealth = 2;
    private int currentHealth;
    private int attackPower = 1;
    private int defense;
    private float moveSpeed = 2.5f;
    private int level = 1;
    private bool isBoss;
    private bool isElite;
    private bool spawnsWeb;
    private float webTimer;
    private string bossName;
    private float dashTimer;
    private float dashRemaining;
    private bool dashing;
    private float shockTimer;
    private float missileTimer;
    private bool touchingPlayer;
    private PlayerHealth contactPlayerHealth;
    private float attackCooldown;
    private float attackInterval = 1f;
    private Vector3 cachedSeparation;
    private int separationPhase;
    private static readonly Collider[] separationBuffer = new Collider[12];

    private Transform targetTransform;
    private HealthBar healthBar;
    private EnemySprite enemySprite;
    private EnemySpawner spawner;

    private void Awake() {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();
        enemySprite = GetComponentInChildren<EnemySprite>();
        if (healthBar != null) {
            healthBar.SetRatio(1f);
        }
        separationPhase = Mathf.Abs(GetInstanceID()) % 3;
        ActiveCount += 1;
    }

    public void ApplyDefinition(EnemyDefinition definition) {
        maxHealth = definition.health;
        currentHealth = definition.health;
        attackPower = definition.attack;
        defense = definition.defense;
        moveSpeed = definition.moveSpeed;
        transform.localScale = new Vector3(definition.scale, definition.scale, definition.scale);
        if (enemySprite != null) {
            enemySprite.SetSheet(definition.spriteResourcePath);
        }
        if (healthBar != null) {
            healthBar.SetRatio(1f);
        }
        isBoss = definition.isBoss;
        isElite = definition.isElite;
        spawnsWeb = definition.spawnsWeb;
        webTimer = 4f;
        bossName = definition.displayName;
        if (isBoss) {
            if (healthBar != null) {
                healthBar.gameObject.SetActive(false);
            }
            dashTimer = 4f;
            shockTimer = 5f;
            missileTimer = 3f;
            if (BossBar.Instance != null) {
                BossBar.Instance.Show(this, bossName);
            }
        }
    }

    public void SetTarget(Transform target) {
        targetTransform = target;
    }

    public void SetSpawner(EnemySpawner value) {
        spawner = value;
    }

    public void SetLevel(int value) {
        level = value;
    }

    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        HandleContactDamage();
        if (spawnsWeb) {
            HandleWeb();
        }
        if (targetTransform == null) {
            return;
        }
        var currentPosition = transform.position;
        var targetPosition = targetTransform.position;
        var moveDirection = targetPosition - currentPosition;
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;
        if (!isBoss && (Time.frameCount + separationPhase) % 3 == 0) {
            cachedSeparation = ComputeSeparation();
        }
        var desiredDirection = moveDirection + cachedSeparation;
        if (desiredDirection.sqrMagnitude > 0.0001f) {
            desiredDirection = desiredDirection.normalized;
        }
        else {
            desiredDirection = moveDirection;
        }
        var currentSpeed = moveSpeed;
        if (isBoss) {
            currentSpeed = UpdateBossPattern();
        }
        transform.position = currentPosition + desiredDirection * currentSpeed * Time.deltaTime;
    }

    private Vector3 ComputeSeparation() {
        var personalSpace = 0.8f * transform.localScale.x;
        var selfPosition = transform.position;
        var count = Physics.OverlapSphereNonAlloc(selfPosition, personalSpace, separationBuffer);
        var push = Vector3.zero;
        for (var index = 0; index < count; index++) {
            var other = separationBuffer[index].GetComponentInParent<Enemy>();
            if (other == null || other == this) {
                continue;
            }
            var away = selfPosition - other.transform.position;
            away.y = 0f;
            var distance = away.magnitude;
            if (distance > 0.001f && distance < personalSpace) {
                push += away.normalized * ((personalSpace - distance) / personalSpace);
            }
        }
        return push;
    }

    private float UpdateBossPattern() {
        var deltaTime = Time.deltaTime;
        var speed = moveSpeed;
        if (dashing) {
            dashRemaining -= deltaTime;
            speed = moveSpeed * 2.6f;
            if (dashRemaining <= 0f) {
                dashing = false;
            }
        }
        else {
            dashTimer -= deltaTime;
            if (dashTimer <= 0f) {
                dashTimer = 4f;
                dashing = true;
                dashRemaining = 0.6f;
            }
        }
        shockTimer -= deltaTime;
        if (shockTimer <= 0f) {
            shockTimer = 5f;
            EmitShockwave();
        }
        missileTimer -= deltaTime;
        if (missileTimer <= 0f) {
            missileTimer = 3f;
            FireBossMissiles();
        }
        return speed;
    }

    private void FireBossMissiles() {
        if (targetTransform == null) {
            return;
        }
        for (var index = 0; index < 3; index++) {
            var missile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            missile.name = "BossMissile";
            var missilePosition = transform.position;
            missilePosition.y = 0.4f;
            var angle = (index - 1) * 25f;
            var offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * 0.8f;
            missile.transform.position = missilePosition + offset;
            missile.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            var missileRenderer = missile.GetComponent<Renderer>();
            if (missileRenderer != null) {
                missileRenderer.material.SetColor("_BaseColor", new Color(0.6f, 0.1f, 0.7f));
                missileRenderer.material.EnableKeyword("_EMISSION");
                missileRenderer.material.SetColor("_EmissionColor", new Color(0.6f, 0.1f, 0.8f));
            }
            var missileCollider = missile.GetComponent<SphereCollider>();
            if (missileCollider != null) {
                missileCollider.isTrigger = true;
            }
            var missileBody = missile.AddComponent<Rigidbody>();
            missileBody.isKinematic = true;
            missileBody.useGravity = false;
            var bossMissile = missile.AddComponent<BossMissile>();
            bossMissile.Configure(targetTransform, attackPower);
        }
    }

    private void EmitShockwave() {
        var shockRadius = 3f * transform.localScale.x;
        var hits = Physics.OverlapSphere(transform.position, shockRadius);
        for (var index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Player")) {
                continue;
            }
            var playerHealth = hits[index].GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.ApplyHit(attackPower);
            }
        }
    }

    public void ApplyHit(int incomingAttack) {
        var damage = CombatFormula.ComputeDamage(incomingAttack, defense);
        currentHealth -= damage;
        CombatText.Show(transform.position, damage, Color.white);
        if (healthBar != null) {
            var ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
        if (isBoss && BossBar.Instance != null) {
            BossBar.Instance.SetRatio(this, (float)currentHealth / maxHealth);
        }
        if (currentHealth <= 0) {
            Die();
        }
    }

    private void Die() {
        if (GameManager.Instance != null) {
            GameManager.Instance.AddKill();
            GameManager.Instance.AddMoney(1 * level);
        }
        if (isBoss && BossBar.Instance != null) {
            BossBar.Instance.Hide(this);
        }
        if (isElite) {
            SpawnRewardItem();
        }
        SpawnDeathSplat();
        Object.Destroy(gameObject);
    }

    private void SpawnRewardItem() {
        var reward = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        reward.name = "RewardItem";
        var rewardPosition = transform.position;
        rewardPosition.y = 0.5f;
        reward.transform.position = rewardPosition;
        reward.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        var rewardRenderer = reward.GetComponent<Renderer>();
        if (rewardRenderer != null) {
            rewardRenderer.material.SetColor("_BaseColor", new Color(1f, 0.85f, 0.2f));
            rewardRenderer.material.EnableKeyword("_EMISSION");
            rewardRenderer.material.SetColor("_EmissionColor", new Color(1f, 0.7f, 0.1f));
        }
        var rewardCollider = reward.GetComponent<SphereCollider>();
        if (rewardCollider != null) {
            rewardCollider.isTrigger = true;
            rewardCollider.radius = 1.5f;
        }
        var rewardBody = reward.AddComponent<Rigidbody>();
        rewardBody.isKinematic = true;
        rewardBody.useGravity = false;
        reward.AddComponent<RewardItem>();
    }

    private void SpawnDeathSplat() {
        var splatObject = new GameObject("DeathSplat");
        var splatPosition = transform.position;
        splatPosition.y = 0.06f;
        splatObject.transform.position = splatPosition;
        splatObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        var splatScale = transform.localScale.x * 2f;
        splatObject.transform.localScale = new Vector3(splatScale, splatScale, splatScale);
        splatObject.AddComponent<SpriteRenderer>();
        splatObject.AddComponent<DeathSplat>();
    }

    private void HandleWeb() {
        webTimer -= Time.deltaTime;
        if (webTimer <= 0f) {
            webTimer = 4f;
            SpawnWeb();
        }
    }

    private void SpawnWeb() {
        var web = new GameObject("WebZone");
        var webPosition = transform.position;
        webPosition.y = 0.05f;
        web.transform.position = webPosition;
        web.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        web.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        web.AddComponent<SpriteRenderer>();
        var webCollider = web.AddComponent<SphereCollider>();
        webCollider.isTrigger = true;
        webCollider.radius = 0.5f;
        var webBody = web.AddComponent<Rigidbody>();
        webBody.isKinematic = true;
        webBody.useGravity = false;
        web.AddComponent<WebZone>();
    }

    private void HandleContactDamage() {
        if (!touchingPlayer || contactPlayerHealth == null) {
            return;
        }
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f) {
            attackCooldown = attackInterval;
            contactPlayerHealth.ApplyHit(attackPower);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) {
            return;
        }
        contactPlayerHealth = playerHealth;
        touchingPlayer = true;
        playerHealth.ApplyHit(attackPower);
        attackCooldown = attackInterval;
    }

    private void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        touchingPlayer = false;
        contactPlayerHealth = null;
    }

    private void OnDestroy() {
        ActiveCount -= 1;
        if (ActiveCount < 0) {
            ActiveCount = 0;
        }
        if (spawner != null) {
            spawner.NotifyEnemyRemoved();
        }
    }
}
