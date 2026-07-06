using UnityEngine;

public class BossIndicator : MonoBehaviour {
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float edgeMargin = 60f;

    private RectTransform canvasRect;

    void Awake() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
        if (arrow != null) {
            canvasRect = arrow.parent as RectTransform;
            arrow.gameObject.SetActive(false);
        }
    }

    void Update() {
        if (arrow == null || canvasRect == null || BossBar.Instance == null) {
            return;
        }
        if (mainCamera == null) {
            mainCamera = Camera.main;
            if (mainCamera == null) {
                return;
            }
        }
        Transform bossTransform = BossBar.Instance.GetCurrentBossTransform();
        if (bossTransform == null) {
            if (arrow.gameObject.activeSelf) {
                arrow.gameObject.SetActive(false);
            }
            return;
        }
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(bossTransform.position);
        bool onScreen = viewportPoint.z > 0f && viewportPoint.x > 0.02f && viewportPoint.x < 0.98f && viewportPoint.y > 0.02f && viewportPoint.y < 0.98f;
        if (onScreen) {
            if (arrow.gameObject.activeSelf) {
                arrow.gameObject.SetActive(false);
            }
            return;
        }
        if (!arrow.gameObject.activeSelf) {
            arrow.gameObject.SetActive(true);
        }
        Vector2 direction = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);
        if (viewportPoint.z < 0f) {
            direction = -direction;
        }
        if (direction.sqrMagnitude < 0.0001f) {
            direction = Vector2.up;
        }
        direction = direction.normalized;
        Rect rect = canvasRect.rect;
        float halfWidth = rect.width * 0.5f - edgeMargin;
        float halfHeight = rect.height * 0.5f - edgeMargin;
        float scaleX = Mathf.Abs(direction.x) > 0.0001f ? halfWidth / Mathf.Abs(direction.x) : float.MaxValue;
        float scaleY = Mathf.Abs(direction.y) > 0.0001f ? halfHeight / Mathf.Abs(direction.y) : float.MaxValue;
        float edgeScale = Mathf.Min(scaleX, scaleY);
        arrow.anchoredPosition = direction * edgeScale;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrow.localEulerAngles = new Vector3(0f, 0f, angle);
    }
}
