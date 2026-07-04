using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.25f;
    [SerializeField] private float height = 20f;

    private Vector3 followVelocity;

    void LateUpdate() {
        if (target == null) {
            return;
        }
        Vector3 targetPosition = target.position;
        Vector3 desiredPosition = new Vector3(targetPosition.x, height, targetPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, smoothTime);
    }
}
