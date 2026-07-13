using UnityEngine;

public class HealthBar : MonoBehaviour {
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.55f, 0.75f);
    [SerializeField] private float barWidth = 1.2f;

    private Transform anchorTransform;
    private Transform fillTransform;
    private Transform backgroundTransform;
    private static readonly Quaternion flatRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Awake() {
        anchorTransform = transform.parent;
        fillTransform = transform.Find("Fill");
        backgroundTransform = transform.Find("Background");
    }

    public void SetRatio(float ratio) {
        if (fillTransform == null) {
            fillTransform = transform.Find("Fill");
        }
        if (backgroundTransform == null) {
            backgroundTransform = transform.Find("Background");
        }
        var clampedRatio = Mathf.Clamp01(ratio);
        var visible = ratio > 0f && ratio < 1f;
        if (backgroundTransform != null) {
            backgroundTransform.gameObject.SetActive(visible);
        }
        if (fillTransform != null) {
            fillTransform.gameObject.SetActive(visible);
            var fillScale = fillTransform.localScale;
            fillScale.x = barWidth * clampedRatio;
            fillTransform.localScale = fillScale;
            var fillPosition = fillTransform.localPosition;
            fillPosition.x = -barWidth * 0.5f * (1f - clampedRatio);
            fillTransform.localPosition = fillPosition;
        }
    }

    private void LateUpdate() {
        if (anchorTransform == null) {
            return;
        }
        transform.position = anchorTransform.position + Vector3.Scale(worldOffset, anchorTransform.lossyScale);
        transform.rotation = flatRotation;
        var parentScale = anchorTransform.lossyScale.x;
        if (parentScale > 0.0001f) {
            var inverseScale = 1f / parentScale;
            transform.localScale = new Vector3(inverseScale, inverseScale, inverseScale);
        }
    }
}
