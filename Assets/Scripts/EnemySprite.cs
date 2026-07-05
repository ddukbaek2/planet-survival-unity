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

    void Awake() {
        anchorTransform = transform.parent;
        LoadBaseSprite();
        if (anchorTransform != null) {
            lastPosition = anchorTransform.position;
        }
    }

    void LoadBaseSprite() {
        if (spriteRenderer == null) {
            return;
        }
        spriteRenderer.flipX = false;
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (int index = 0; index < loadedSprites.Length; index++) {
            string spriteName = loadedSprites[index].name;
            int underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            string numberText = spriteName.Substring(underscoreIndex + 1);
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

    void LateUpdate() {
        if (anchorTransform == null || spriteRenderer == null) {
            return;
        }
        Vector3 currentPosition = anchorTransform.position;
        Vector3 moveDelta = currentPosition - lastPosition;
        moveDelta.y = 0f;
        lastPosition = currentPosition;
        if (moveDelta.magnitude > moveThreshold) {
            currentYaw = Mathf.Atan2(moveDelta.x, moveDelta.z) * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.AngleAxis(currentYaw, Vector3.up) * flatRotation;
    }
}
