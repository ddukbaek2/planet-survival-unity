using UnityEngine;

public class OrbitProjectile : MonoBehaviour {
    private Transform centerTransform;
    private float radius = 2.6f;
    private float angularSpeed = 200f;
    private int damage = 1;
    private float lifeTime = 4f;
    private float currentAngle;

    public void Configure(Transform center, float orbitRadius, float degreesPerSecond, int damageAmount, float duration, float startAngle) {
        centerTransform = center;
        radius = orbitRadius;
        angularSpeed = degreesPerSecond;
        damage = damageAmount;
        lifeTime = duration;
        currentAngle = startAngle;
    }

    void Start() {
        Object.Destroy(gameObject, lifeTime);
    }

    void Update() {
        if (centerTransform == null) {
            return;
        }
        currentAngle += angularSpeed * Time.deltaTime;
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * radius;
        transform.position = centerTransform.position + offset;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) {
            return;
        }
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.TakeDamage(damage);
        }
    }
}
