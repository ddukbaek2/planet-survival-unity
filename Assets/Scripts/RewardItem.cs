using UnityEngine;

public class RewardItem : MonoBehaviour {
    private void Update() {
        transform.Rotate(0f, 120f * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        if (GameManager.Instance != null) {
            GameManager.Instance.GrantLevelUpReward();
        }
        Object.Destroy(gameObject);
    }
}
