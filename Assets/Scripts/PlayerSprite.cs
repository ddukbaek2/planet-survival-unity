using UnityEngine;

public class PlayerSprite : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string resourcePath = "Sprites/marine_blue_sheet_1";
    [SerializeField] private int baseFrameIndex = 16;

    private Transform anchorTransform;
    private float currentYaw;

    private static readonly Quaternion flatRotation = Quaternion.Euler(90f, 0f, 0f);

    private void Awake() {
        anchorTransform = transform.parent;
        LoadBaseSprite();
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

    private void LateUpdate() {
        if (anchorTransform == null || spriteRenderer == null) {
            return;
        }
        var facingDirection = anchorTransform.forward;
        facingDirection.y = 0f;
        if (facingDirection.sqrMagnitude > 0.0001f) {
            currentYaw = Mathf.Atan2(facingDirection.x, facingDirection.z) * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.AngleAxis(currentYaw, Vector3.up) * flatRotation;
    }
}
