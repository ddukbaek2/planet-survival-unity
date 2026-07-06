using UnityEngine;

public class RewardItem : MonoBehaviour {
    void Update() {
        transform.Rotate(0f, 120f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        if (GameManager.Instance != null) {
            GameManager.Instance.GrantLevelUpReward();
        }
        Object.Destroy(gameObject);
    }
}
