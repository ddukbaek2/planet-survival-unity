using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : MonoBehaviour {
    private int damage = 1;
    private float maxRadius = 6f;
    private float duration = 0.3f;
    private float elapsedTime;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void Configure(int damageAmount, float radius, float sweepDuration) {
        damage = damageAmount;
        maxRadius = radius;
        duration = sweepDuration;
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);
        float currentRadius = maxRadius * progress;
        float diameter = currentRadius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
        for (int index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Enemy")) {
                continue;
            }
            Enemy enemy = hits[index].GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy)) {
                hitEnemies.Add(enemy);
                enemy.TakeDamage(damage);
            }
        }
        if (progress >= 1f) {
            Object.Destroy(gameObject);
        }
    }
}
