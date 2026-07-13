using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float height = 20f;
    [SerializeField] private float followSmoothTime = 0.12f;

    private Vector3 followVelocity;

    private void LateUpdate() {
        if (target == null) {
            return;
        }
        var targetPosition = target.position;
        var desiredPosition = new Vector3(targetPosition.x, height, targetPosition.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref followVelocity, followSmoothTime);
    }
}
