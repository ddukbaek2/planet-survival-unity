using UnityEngine;

public class BossIndicator : MonoBehaviour {
    [SerializeField] private RectTransform arrow;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float edgeMargin = 60f;

    private RectTransform canvasRect;

    private void Awake() {
        if (mainCamera == null) {
            mainCamera = Camera.main;
        }
        if (arrow != null) {
            canvasRect = arrow.parent as RectTransform;
            arrow.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (arrow == null || canvasRect == null || BossBar.Instance == null) {
            return;
        }
        if (mainCamera == null) {
            mainCamera = Camera.main;
            if (mainCamera == null) {
                return;
            }
        }
        var bossTransform = BossBar.Instance.GetCurrentBossTransform();
        if (bossTransform == null) {
            if (arrow.gameObject.activeSelf) {
                arrow.gameObject.SetActive(false);
            }
            return;
        }
        var viewportPoint = mainCamera.WorldToViewportPoint(bossTransform.position);
        var onScreen = viewportPoint.z > 0f && viewportPoint.x > 0.02f && viewportPoint.x < 0.98f && viewportPoint.y > 0.02f && viewportPoint.y < 0.98f;
        if (onScreen) {
            if (arrow.gameObject.activeSelf) {
                arrow.gameObject.SetActive(false);
            }
            return;
        }
        if (!arrow.gameObject.activeSelf) {
            arrow.gameObject.SetActive(true);
        }
        var direction = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);
        if (viewportPoint.z < 0f) {
            direction = -direction;
        }
        if (direction.sqrMagnitude < 0.0001f) {
            direction = Vector2.up;
        }
        direction = direction.normalized;
        var rect = canvasRect.rect;
        var halfWidth = rect.width * 0.5f - edgeMargin;
        var halfHeight = rect.height * 0.5f - edgeMargin;
        var scaleX = Mathf.Abs(direction.x) > 0.0001f ? halfWidth / Mathf.Abs(direction.x) : float.MaxValue;
        var scaleY = Mathf.Abs(direction.y) > 0.0001f ? halfHeight / Mathf.Abs(direction.y) : float.MaxValue;
        var edgeScale = Mathf.Min(scaleX, scaleY);
        arrow.anchoredPosition = direction * edgeScale;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrow.localEulerAngles = new Vector3(0f, 0f, angle);
    }
}
