using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float height = 20f;
    [SerializeField] private float mapHalfX = 75f;
    [SerializeField] private float mapHalfZ = 75f;

    private Camera cameraComponent;
    private Vector3 followVelocity;

    void Awake() {
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate() {
        if (target == null) {
            return;
        }
        float halfHeight = cameraComponent.orthographicSize;
        float halfWidth = halfHeight * cameraComponent.aspect;
        float clampX = Mathf.Max(0f, mapHalfX - halfWidth);
        float clampZ = Mathf.Max(0f, mapHalfZ - halfHeight);
        Vector3 targetPosition = target.position;
        float desiredX = Mathf.Clamp(targetPosition.x, -clampX, clampX);
        float desiredZ = Mathf.Clamp(targetPosition.z, -clampZ, clampZ);
        Vector3 desiredPosition = new Vector3(desiredX, height, desiredZ);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);
    }
}
