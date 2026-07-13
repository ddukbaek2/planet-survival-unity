using UnityEngine;

public class EnemySprite : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string resourcePath = "Sprites/roach_sheet";
    [SerializeField] private int baseFrameIndex = 16;
    [SerializeField] private float moveThreshold = 0.01f;

    private Transform anchorTransform;
    private Vector3 lastPosition;
    private float currentYaw;

    private static readonly Quaternion flatRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Awake() {
        anchorTransform = transform.parent;
        LoadBaseSprite();
        if (anchorTransform != null) {
            lastPosition = anchorTransform.position;
        }
    }

    private void LoadBaseSprite() {
        if (spriteRenderer == null) {
            return;
        }
        spriteRenderer.flipX = false;
        var loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (var index = 0; index < loadedSprites.Length; index++) {
            var spriteName = loadedSprites[index].name;
            var underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            var numberText = spriteName.Substring(underscoreIndex + 1);
            int frameIndex;
            if (int.TryParse(numberText, out frameIndex) && frameIndex == baseFrameIndex) {
                spriteRenderer.sprite = loadedSprites[index];
                return;
            }
        }
    }

    public void SetSheet(string newResourcePath) {
        if (newResourcePath == resourcePath) {
            return;
        }
        resourcePath = newResourcePath;
        LoadBaseSprite();
    }

    private void LateUpdate() {
        if (anchorTransform == null || spriteRenderer == null) {
            return;
        }
        var currentPosition = anchorTransform.position;
        var moveDelta = currentPosition - lastPosition;
        moveDelta.y = 0f;
        lastPosition = currentPosition;
        if (moveDelta.magnitude > moveThreshold) {
            currentYaw = Mathf.Atan2(moveDelta.x, moveDelta.z) * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.AngleAxis(currentYaw, Vector3.up) * flatRotation;
    }
}
