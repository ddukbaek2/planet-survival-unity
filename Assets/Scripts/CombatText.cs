using UnityEngine;

public class CombatText : MonoBehaviour {
    private static CombatText instance;

    [SerializeField] private GameObject floatingTextPrefab;

    void Awake() {
        instance = this;
    }

    void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    public static void Show(Vector3 worldPosition, int amount, Color color) {
        if (instance == null || instance.floatingTextPrefab == null) {
            return;
        }
        instance.Spawn(worldPosition, amount.ToString(), color);
    }

    void Spawn(Vector3 worldPosition, string content, Color color) {
        Vector3 spawnPosition = worldPosition + new Vector3(Random.Range(-0.3f, 0.3f), 0.3f, 0f);
        GameObject textObject = Object.Instantiate(floatingTextPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        FloatingText floatingText = textObject.GetComponent<FloatingText>();
        if (floatingText != null) {
            floatingText.Show(content, color);
        }
    }
}
