using UnityEngine;

public class Projectile : MonoBehaviour {
    public enum ProjectileMode {
        Straight,
        Homing,
        Boomerang
    }

    private float speed = 18f;
    private float lifeTime = 3f;
    private int attackPower = 1;
    private int pierceRemaining;
    private float explosionRadius;
    private ProjectileMode mode = ProjectileMode.Straight;

    private Vector3 moveDirection = Vector3.forward;
    private float elapsedTime;
    private bool boomerangReversed;
    private Transform homingTarget;
    private ProjectilePool pool;

    private const float HomingTurnDegreesPerSecond = 240f;

    public void SetPool(ProjectilePool value) {
        pool = value;
    }

    public void Configure(Vector3 direction, int attack, float projectileSpeed, float projectileLifeTime, ProjectileMode projectileMode, int pierce, float explosion) {
        moveDirection = direction.normalized;
        attackPower = attack;
        speed = projectileSpeed;
        lifeTime = projectileLifeTime;
        mode = projectileMode;
        pierceRemaining = pierce;
        explosionRadius = explosion;
        elapsedTime = 0f;
        boomerangReversed = false;
        homingTarget = null;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void SetDirection(Vector3 direction) {
        moveDirection = direction.normalized;
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifeTime) {
            ReleaseSelf();
            return;
        }
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

    private void UpdateHoming() {
        if (homingTarget == null) {
            homingTarget = FindNearestEnemy();
        }
        if (homingTarget == null) {
            return;
        }
        var desiredDirection = homingTarget.position - transform.position;
        desiredDirection.y = 0f;
        if (desiredDirection.sqrMagnitude < 0.001f) {
            return;
        }
        desiredDirection = desiredDirection.normalized;
        var maxRadians = HomingTurnDegreesPerSecond * Mathf.Deg2Rad * Time.deltaTime;
        moveDirection = Vector3.RotateTowards(moveDirection, desiredDirection, maxRadians, 0f).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    private Transform FindNearestEnemy() {
        var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Transform nearest = null;
        var nearestDistance = float.MaxValue;
        var currentPosition = transform.position;
        for (var index = 0; index < enemies.Length; index++) {
            var distance = (enemies[index].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearest = enemies[index].transform;
            }
        }
        return nearest;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) {
            return;
        }
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.ApplyHit(attackPower);
        }
        if (explosionRadius > 0f) {
            Explode();
        }
        if (pierceRemaining > 0) {
            pierceRemaining -= 1;
            return;
        }
        ReleaseSelf();
    }

    private void Explode() {
        var hits = Physics.OverlapSphere(transform.position, explosionRadius);
        for (var index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Enemy")) {
                continue;
            }
            var enemy = hits[index].GetComponent<Enemy>();
            if (enemy != null) {
                enemy.ApplyHit(attackPower);
            }
        }
    }

    private void ReleaseSelf() {
        if (pool != null) {
            pool.Release(this);
        }
        else {
            Object.Destroy(gameObject);
        }
    }
}
