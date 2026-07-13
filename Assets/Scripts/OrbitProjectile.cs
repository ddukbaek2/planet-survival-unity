using UnityEngine;

public class OrbitProjectile : MonoBehaviour {
    private Transform centerTransform;
    private float radius = 2.6f;
    private float angularSpeed = 200f;
    private int attackPower = 1;
    private float lifeTime = 4f;
    private float currentAngle;

    public void Configure(Transform center, float orbitRadius, float degreesPerSecond, int attack, float duration, float startAngle) {
        centerTransform = center;
        radius = orbitRadius;
        angularSpeed = degreesPerSecond;
        attackPower = attack;
        lifeTime = duration;
        currentAngle = startAngle;
    }

    private void Start() {
        Object.Destroy(gameObject, lifeTime);
    }

    private void Update() {
        if (centerTransform == null) {
            return;
        }
        currentAngle += angularSpeed * Time.deltaTime;
        var radians = currentAngle * Mathf.Deg2Rad;
        var offset = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians)) * radius;
        transform.position = centerTransform.position + offset;
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) {
            return;
        }
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.ApplyHit(attackPower);
        }
    }
}
