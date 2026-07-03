using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float speed = 18f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 1;

    private Vector3 moveDirection = Vector3.forward;

    public void SetDirection(Vector3 direction) {
        moveDirection = direction.normalized;
    }

    void Start() {
        Object.Destroy(gameObject, lifeTime);
    }

    void Update() {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) {
            return;
        }
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null) {
            enemy.TakeDamage(damage);
        }
        Object.Destroy(gameObject);
    }
}
