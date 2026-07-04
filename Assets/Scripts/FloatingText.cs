using UnityEngine;

public class FloatingText : MonoBehaviour {
    [SerializeField] private float riseSpeed = 3.2f;
    [SerializeField] private float lifeTime = 0.7f;

    private float elapsedTime;
    private TMPro.TextMeshPro textMesh;
    private Color baseColor = Color.white;

    void Awake() {
        textMesh = GetComponent<TMPro.TextMeshPro>();
    }

    public void Show(string content, Color color) {
        if (textMesh == null) {
            textMesh = GetComponent<TMPro.TextMeshPro>();
        }
        baseColor = color;
        elapsedTime = 0f;
        if (textMesh != null) {
            textMesh.text = content;
            textMesh.color = color;
        }
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        transform.position += new Vector3(0f, 0f, riseSpeed * Time.deltaTime);
        float progress = Mathf.Clamp01(elapsedTime / lifeTime);
        if (textMesh != null) {
            Color currentColor = baseColor;
            currentColor.a = 1f - progress;
            textMesh.color = currentColor;
        }
        if (elapsedTime >= lifeTime) {
            Object.Destroy(gameObject);
        }
    }
}
