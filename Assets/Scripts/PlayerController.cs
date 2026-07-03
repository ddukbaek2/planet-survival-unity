using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float boundX = 31.5f;
    [SerializeField] private float boundZ = 18.9f;
    [SerializeField] private float dragDeadzonePixels = 12f;

    private bool isDragging;
    private Vector2 dragOrigin;

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        Pointer pointer = Pointer.current;
        if (pointer == null) {
            return;
        }
        bool isPressed = pointer.press.isPressed;
        if (!isPressed) {
            isDragging = false;
            return;
        }
        Vector2 pointerPosition = pointer.position.ReadValue();
        if (!isDragging) {
            isDragging = true;
            dragOrigin = pointerPosition;
            return;
        }
        Vector2 dragVector = pointerPosition - dragOrigin;
        if (dragVector.magnitude < dragDeadzonePixels) {
            return;
        }
        Vector2 dragDirection = dragVector.normalized;
        Vector3 moveDirection = new Vector3(dragDirection.x, 0f, dragDirection.y);
        transform.rotation = Quaternion.LookRotation(moveDirection);
        Vector3 nextPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, -boundX, boundX);
        nextPosition.z = Mathf.Clamp(nextPosition.z, -boundZ, boundZ);
        nextPosition.y = transform.position.y;
        transform.position = nextPosition;
    }
}
