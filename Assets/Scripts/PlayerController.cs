using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float dragDeadzonePixels = 12f;
    [SerializeField] private RectTransform joystickBase;
    [SerializeField] private RectTransform joystickKnob;
    [SerializeField] private float joystickRadius = 80f;

    private bool isDragging;
    private Vector2 dragOrigin;
    private int slowSourceCount;

    public void AddSlow() {
        slowSourceCount += 1;
    }

    public void RemoveSlow() {
        slowSourceCount -= 1;
        if (slowSourceCount < 0) {
            slowSourceCount = 0;
        }
    }

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
        }
        Vector2 dragVector = pointerPosition - dragOrigin;
        if (dragVector.magnitude > joystickRadius) {
            dragVector = dragVector.normalized * joystickRadius;
        }
        UpdateJoystickVisual(dragVector);
        if (dragVector.magnitude < dragDeadzonePixels) {
            return;
        }
        Vector2 dragDirection = dragVector.normalized;
        Vector3 moveDirection = new Vector3(dragDirection.x, 0f, dragDirection.y);
        transform.rotation = Quaternion.LookRotation(moveDirection);
        float effectiveSpeed = slowSourceCount > 0 ? moveSpeed * 0.5f : moveSpeed;
        Vector3 nextPosition = transform.position + moveDirection * effectiveSpeed * Time.deltaTime;
        nextPosition.y = transform.position.y;
        transform.position = nextPosition;
    }

    void UpdateJoystickVisual(Vector2 dragVector) {
        if (joystickBase == null) {
            return;
        }
        if (!joystickBase.gameObject.activeSelf) {
            joystickBase.gameObject.SetActive(true);
        }
        joystickBase.position = new Vector3(dragOrigin.x, dragOrigin.y, 0f);
        if (joystickKnob != null) {
            Vector2 knobPosition = dragOrigin + dragVector;
            joystickKnob.position = new Vector3(knobPosition.x, knobPosition.y, 0f);
        }
    }

    void HideJoystick() {
        if (joystickBase != null && joystickBase.gameObject.activeSelf) {
            joystickBase.gameObject.SetActive(false);
        }
    }
}
