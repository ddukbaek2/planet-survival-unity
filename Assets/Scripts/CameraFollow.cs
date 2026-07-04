using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float height = 20f;
    [SerializeField] private float edgeMargin = 3f;
    [SerializeField] private float followSmoothTime = 0.12f;

    private Camera cameraComponent;
    private Vector3 followVelocity;

    void Awake() {
        cameraComponent = GetComponent<Camera>();
    }

    void LateUpdate() {
        if (target == null) {
            return;
        }
        if (cameraComponent == null) {
            cameraComponent = GetComponent<Camera>();
        }
        float halfHeight = cameraComponent.orthographicSize;
        float halfWidth = halfHeight * cameraComponent.aspect;
        float deadzoneX = Mathf.Max(0.5f, halfWidth - edgeMargin);
        float deadzoneZ = Mathf.Max(0.5f, halfHeight - edgeMargin);

        Vector3 cameraPosition = transform.position;
        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = cameraPosition;

        float offsetX = targetPosition.x - cameraPosition.x;
        if (offsetX > deadzoneX) {
            desiredPosition.x = targetPosition.x - deadzoneX;
        }
        else if (offsetX < -deadzoneX) {
            desiredPosition.x = targetPosition.x + deadzoneX;
        }

        float offsetZ = targetPosition.z - cameraPosition.z;
        if (offsetZ > deadzoneZ) {
            desiredPosition.z = targetPosition.z - deadzoneZ;
        }
        else if (offsetZ < -deadzoneZ) {
            desiredPosition.z = targetPosition.z + deadzoneZ;
        }

        desiredPosition.y = height;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);
    }
}
