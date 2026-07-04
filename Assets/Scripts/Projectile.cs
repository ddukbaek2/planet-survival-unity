using UnityEngine;

public class Projectile : MonoBehaviour {
    public enum ProjectileMode {
        Straight,
        Homing,
        Boomerang
    }

    private float speed = 18f;
    private float lifeTime = 3f;
    private int damage = 1;
    private int pierceRemaining;
    private float explosionRadius;
    private ProjectileMode mode = ProjectileMode.Straight;

    private Vector3 moveDirection = Vector3.forward;
    private float elapsedTime;
    private bool boomerangReversed;
    private Transform homingTarget;

    private const float HomingTurnDegreesPerSecond = 240f;

    public void Configure(Vector3 direction, int damageAmount, float projectileSpeed, float projectileLifeTime, ProjectileMode projectileMode, int pierce, float explosion) {
        moveDirection = direction.normalized;
        damage = damageAmount;
        speed = projectileSpeed;
        lifeTime = projectileLifeTime;
        mode = projectileMode;
        pierceRemaining = pierce;
        explosionRadius = explosion;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void SetDirection(Vector3 direction) {
        moveDirection = direction.normalized;
    }

    void Start() {
        Object.Destroy(gameObject, lifeTime);
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        if (mode == ProjectileMode.Homing) {
            UpdateHoming();
        }
        else if (mode == ProjectileMode.Boomerang) {
            if (!boomerangReversed && elapsedTime > lifeTime * 0.5f) {
                boomerangReversed = true;
                moveDirection = -moveDirection;
                transform.rotation = Quaternion.LookRotation(moveDirection);
            }
        }
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void UpdateHoming() {
        if (homingTarget == null) {
            homingTarget = FindNearestEnemy();
        }
        if (homingTarget == null) {
            return;
        }
        Vector3 desiredDirection = homingTarget.position - transform.position;
        desiredDirection.y = 0f;
        if (desiredDirection.sqrMagnitude < 0.001f) {
            return;
        }
        desiredDirection = desiredDirection.normalized;
        float maxRadians = HomingTurnDegreesPerSecond * Mathf.Deg2Rad * Time.deltaTime;
        moveDirection = Vector3.RotateTowards(moveDirection, desiredDirection, maxRadians, 0f).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    Transform FindNearestEnemy() {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        Vector3 currentPosition = transform.position;
        for (int index = 0; index < enemies.Length; index++) {
            float distance = (enemies[index].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearest = enemies[index].transform;
            }
        }
        return nearest;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) {
            return;
        }
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.TakeDamage(damage);
        }
        if (explosionRadius > 0f) {
            Explode();
        }
        if (pierceRemaining > 0) {
            pierceRemaining -= 1;
            return;
        }
        Object.Destroy(gameObject);
    }

    void Explode() {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Enemy")) {
                continue;
            }
            Enemy enemy = hits[index].GetComponent<Enemy>();
            if (enemy != null) {
                enemy.TakeDamage(damage);
            }
        }
    }
}
