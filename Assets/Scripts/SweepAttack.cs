using System.Collections.Generic;
using UnityEngine;

public class SweepAttack : MonoBehaviour {
    private int attackPower = 1;
    private float maxRadius = 6f;
    private float duration = 0.3f;
    private float elapsedTime;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();

    public void Configure(int attack, float radius, float sweepDuration) {
        attackPower = attack;
        maxRadius = radius;
        duration = sweepDuration;
    }

    private void Update() {
        elapsedTime += Time.deltaTime;
        var progress = Mathf.Clamp01(elapsedTime / duration);
        var currentRadius = maxRadius * progress;
        var diameter = currentRadius * 2f;
        transform.localScale = new Vector3(diameter, diameter, 1f);
        var hits = Physics.OverlapSphere(transform.position, currentRadius);
        for (var index = 0; index < hits.Length; index++) {
            if (!hits[index].CompareTag("Enemy")) {
                continue;
            }
            var enemy = hits[index].GetComponent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy)) {
                hitEnemies.Add(enemy);
                enemy.ApplyHit(attackPower);
            }
        }
        if (progress >= 1f) {
            Object.Destroy(gameObject);
        }
    }
}
