using UnityEngine;

public class MineField : MonoBehaviour {
    private int attackPower = 1;
    private float radius = 2.6f;
    private float duration = 4f;
    private float tickInterval = 0.5f;
    private float lifeTimer;
    private float tickTimer;

    public void Configure(int attack, float fieldRadius, float fieldDuration, float interval) {
        attackPower = attack;
        radius = fieldRadius;
        duration = fieldDuration;
        tickInterval = interval;
        float diameter = radius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    void Update() {
        lifeTimer += Time.deltaTime;
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0f) {
            tickTimer = tickInterval;
            DamageEnemies();
        }
        if (lifeTimer >= duration) {
            Object.Destroy(gameObject);
        }
    }

    void DamageEnemies() {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        for (int index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Enemy")) {
                continue;
            }
            Enemy enemy = hits[index].GetComponent<Enemy>();
            if (enemy != null) {
                enemy.ApplyHit(attackPower);
            }
        }
    }
}
