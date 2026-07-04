using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private float height = 20f;
    [SerializeField] private float deadzoneRadius = 2.5f;

    private Vector3 followVelocity;
    private Vector3 focusPoint;
    private bool focusInitialized;

    void LateUpdate() {
        if (target == null) {
            return;
        }
        Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
        if (!focusInitialized) {
            focusPoint = targetPosition;
            focusInitialized = true;
        }
        Vector3 offset = targetPosition - focusPoint;
        float distance = offset.magnitude;
        if (distance > deadzoneRadius) {
            focusPoint += offset.normalized * (distance - deadzoneRadius);
        }
        Vector3 desiredPosition = new Vector3(focusPoint.x, height, focusPoint.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);
    }
}
