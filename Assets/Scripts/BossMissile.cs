using UnityEngine;

public class BossMissile : MonoBehaviour {
    private Transform target;
    private int damage;
    private float speed = 6f;
    private float lifeTime = 6f;
    private float elapsed;

    public void Configure(Transform targetTransform, int missileDamage) {
        target = targetTransform;
        damage = missileDamage;
    }

    private void Update() {
        elapsed += Time.deltaTime;
        if (elapsed >= lifeTime || target == null) {
            Object.Destroy(gameObject);
            return;
        }
        var direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f) {
            direction = direction.normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null) {
            playerHealth.ApplyHit(damage);
        }
        Object.Destroy(gameObject);
    }
}
