using UnityEngine;

public class CombatText : MonoBehaviour {
    private static CombatText instance;

    [SerializeField] private GameObject floatingTextPrefab;

    private void Awake() {
        instance = this;
    }

    private void OnDestroy() {
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

    private void Spawn(Vector3 worldPosition, string content, Color color) {
        var textObject = Object.Instantiate(floatingTextPrefab, worldPosition, Quaternion.Euler(90f, 0f, 0f));
        var floatingText = textObject.GetComponent<FloatingText>();
        if (floatingText != null) {
            floatingText.Show(content, color);
        }
    }
}
