using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float boundX = 31.5f;
    [SerializeField] private float boundZ = 18.9f;
    [SerializeField] private float dragDeadzonePixels = 12f;
    [SerializeField] private RectTransform joystickBase;
    [SerializeField] private RectTransform joystickKnob;
    [SerializeField] private float joystickRadius = 80f;

    private bool isDragging;
    private Vector2 dragOrigin;

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            HideJoystick();
            return;
        }
        Pointer pointer = Pointer.current;
        if (pointer == null) {
            return;
        }
        bool isPressed = pointer.press.isPressed;
        if (!isPressed) {
            isDragging = false;
            HideJoystick();
            return;
        }
        Vector2 pointerPosition = pointer.position.ReadValue();
        if (!isDragging) {
            isDragging = true;
            dragOrigin = pointerPosition;
            ShowJoystick(dragOrigin);
        }
        Vector2 dragVector = pointerPosition - dragOrigin;
        UpdateJoystickKnob(dragVector);
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

    void ShowJoystick(Vector2 screenPosition) {
        if (joystickBase == null) {
            return;
        }
        joystickBase.gameObject.SetActive(true);
        joystickBase.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
        if (joystickKnob != null) {
            joystickKnob.position = new Vector3(screenPosition.x, screenPosition.y, 0f);
        }
    }

    void UpdateJoystickKnob(Vector2 dragVector) {
        if (joystickKnob == null) {
            return;
        }
        Vector2 clampedVector = Vector2.ClampMagnitude(dragVector, joystickRadius);
        Vector2 knobPosition = dragOrigin + clampedVector;
        joystickKnob.position = new Vector3(knobPosition.x, knobPosition.y, 0f);
    }

    void HideJoystick() {
        if (joystickBase != null && joystickBase.gameObject.activeSelf) {
            joystickBase.gameObject.SetActive(false);
        }
    }
}
