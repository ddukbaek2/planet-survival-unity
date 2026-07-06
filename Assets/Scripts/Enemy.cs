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
    private string bossName;
    private float dashTimer;
    private float dashRemaining;
    private bool dashing;
    private float shockTimer;
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

    void Awake() {
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
        bossName = definition.displayName;
        if (isBoss && BossBar.Instance != null) {
            dashTimer = 4f;
            shockTimer = 5f;
            BossBar.Instance.Show(this, bossName);
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

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        HandleContactDamage();
        if (targetTransform == null) {
            return;
        }
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = targetTransform.position;
        Vector3 moveDirection = targetPosition - currentPosition;
        moveDirection.y = 0f;
        moveDirection = moveDirection.normalized;
        if (!isBoss && (Time.frameCount + separationPhase) % 3 == 0) {
            cachedSeparation = ComputeSeparation();
        }
        Vector3 desiredDirection = moveDirection + cachedSeparation;
        if (desiredDirection.sqrMagnitude > 0.0001f) {
            desiredDirection = desiredDirection.normalized;
        }
        else {
            desiredDirection = moveDirection;
        }
        float currentSpeed = moveSpeed;
        if (isBoss) {
            currentSpeed = UpdateBossPattern();
        }
        transform.position = currentPosition + desiredDirection * currentSpeed * Time.deltaTime;
    }

    Vector3 ComputeSeparation() {
        float personalSpace = 0.8f * transform.localScale.x;
        Vector3 selfPosition = transform.position;
        int count = Physics.OverlapSphereNonAlloc(selfPosition, personalSpace, separationBuffer);
        Vector3 push = Vector3.zero;
        for (int index = 0; index < count; index++) {
            Enemy other = separationBuffer[index].GetComponentInParent<Enemy>();
            if (other == null || other == this) {
                continue;
            }
            Vector3 away = selfPosition - other.transform.position;
            away.y = 0f;
            float distance = away.magnitude;
            if (distance > 0.001f && distance < personalSpace) {
                push += away.normalized * ((personalSpace - distance) / personalSpace);
            }
        }
        return push;
    }

    float UpdateBossPattern() {
        float deltaTime = Time.deltaTime;
        float speed = moveSpeed;
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
        return speed;
    }

    void EmitShockwave() {
        float shockRadius = 3f * transform.localScale.x;
        Collider[] hits = Physics.OverlapSphere(transform.position, shockRadius);
        for (int index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Player")) {
                continue;
            }
            PlayerHealth playerHealth = hits[index].GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                playerHealth.ApplyHit(attackPower);
            }
        }
    }

    public void ApplyHit(int incomingAttack) {
        int damage = CombatFormula.ComputeDamage(incomingAttack, defense);
        currentHealth -= damage;
        CombatText.Show(transform.position, damage, Color.white);
        if (healthBar != null) {
            float ratio = (float)currentHealth / maxHealth;
            healthBar.SetRatio(ratio);
        }
        if (isBoss && BossBar.Instance != null) {
            BossBar.Instance.SetRatio(this, (float)currentHealth / maxHealth);
        }
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
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

    void SpawnRewardItem() {
        GameObject reward = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        reward.name = "RewardItem";
        Vector3 rewardPosition = transform.position;
        rewardPosition.y = 0.5f;
        reward.transform.position = rewardPosition;
        reward.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        Renderer rewardRenderer = reward.GetComponent<Renderer>();
        if (rewardRenderer != null) {
            rewardRenderer.material.SetColor("_BaseColor", new Color(1f, 0.85f, 0.2f));
            rewardRenderer.material.EnableKeyword("_EMISSION");
            rewardRenderer.material.SetColor("_EmissionColor", new Color(1f, 0.7f, 0.1f));
        }
        SphereCollider rewardCollider = reward.GetComponent<SphereCollider>();
        if (rewardCollider != null) {
            rewardCollider.isTrigger = true;
            rewardCollider.radius = 1.5f;
        }
        Rigidbody rewardBody = reward.AddComponent<Rigidbody>();
        rewardBody.isKinematic = true;
        rewardBody.useGravity = false;
        reward.AddComponent<RewardItem>();
    }

    void SpawnDeathSplat() {
        GameObject splatObject = new GameObject("DeathSplat");
        Vector3 splatPosition = transform.position;
        splatPosition.y = 0.06f;
        splatObject.transform.position = splatPosition;
        splatObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        float splatScale = transform.localScale.x * 2f;
        splatObject.transform.localScale = new Vector3(splatScale, splatScale, splatScale);
        splatObject.AddComponent<SpriteRenderer>();
        splatObject.AddComponent<DeathSplat>();
    }

    void HandleContactDamage() {
        if (!touchingPlayer || contactPlayerHealth == null) {
            return;
        }
        attackCooldown -= Time.deltaTime;
        if (attackCooldown <= 0f) {
            attackCooldown = attackInterval;
            contactPlayerHealth.ApplyHit(attackPower);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) {
            return;
        }
        contactPlayerHealth = playerHealth;
        touchingPlayer = true;
        playerHealth.ApplyHit(attackPower);
        attackCooldown = attackInterval;
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        touchingPlayer = false;
        contactPlayerHealth = null;
    }

    void OnDestroy() {
        ActiveCount -= 1;
        if (ActiveCount < 0) {
            ActiveCount = 0;
        }
        if (spawner != null) {
            spawner.NotifyEnemyRemoved();
        }
    }
}
