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
        var diameter = radius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    private void Update() {
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

    private void DamageEnemies() {
        var hits = Physics.OverlapSphere(transform.position, radius);
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
}
