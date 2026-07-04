using UnityEngine;

public class PlayerSprite : MonoBehaviour {
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string resourcePath = "Sprites/marine_blue_sheet_1";
    [SerializeField] private float framesPerSecond = 10f;
    [SerializeField] private float moveThreshold = 0.015f;

    private readonly Sprite[] frames = new Sprite[32];
    private Transform anchorTransform;
    private Vector3 lastPosition;
    private float animationTimer;
    private int animationColumn;

    private static readonly int[] RowForSector = { 4, 3, 2, 1, 0, 7, 6, 5 };
    private static readonly Quaternion flatRotation = Quaternion.Euler(90f, 0f, 0f);

    void Awake() {
        anchorTransform = transform.parent;
        LoadFrames();
        if (anchorTransform != null) {
            lastPosition = anchorTransform.position;
        }
    }

    void LoadFrames() {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (int index = 0; index < loadedSprites.Length; index++) {
            string spriteName = loadedSprites[index].name;
            int underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            string numberText = spriteName.Substring(underscoreIndex + 1);
            int frameIndex;
            if (int.TryParse(numberText, out frameIndex) && frameIndex >= 0 && frameIndex < frames.Length) {
                frames[frameIndex] = loadedSprites[index];
            }
        }
    }

    void LateUpdate() {
        if (anchorTransform == null || spriteRenderer == null) {
            return;
        }
        transform.rotation = flatRotation;

        Vector3 currentPosition = anchorTransform.position;
        float movedDistance = Vector2.Distance(new Vector2(currentPosition.x, currentPosition.z), new Vector2(lastPosition.x, lastPosition.z));
        bool isMoving = movedDistance > moveThreshold;
        lastPosition = currentPosition;

        Vector3 facingDirection = anchorTransform.forward;
        facingDirection.y = 0f;
        int directionRow = GetDirectionRow(facingDirection);

        if (isMoving) {
            animationTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, framesPerSecond);
            while (animationTimer >= frameDuration) {
                animationTimer -= frameDuration;
                animationColumn = (animationColumn + 1) % 4;
            }
        }
        else {
            animationColumn = 0;
            animationTimer = 0f;
        }

        int frameIndex = directionRow * 4 + animationColumn;
        Sprite frame = frames[frameIndex];
        if (frame != null) {
            spriteRenderer.sprite = frame;
        }
    }

    int GetDirectionRow(Vector3 facingDirection) {
        if (facingDirection.sqrMagnitude < 0.0001f) {
            return 0;
        }
        float angle = Mathf.Atan2(-facingDirection.x, facingDirection.z) * Mathf.Rad2Deg;
        int sector = Mathf.RoundToInt(angle / 45f);
        sector = ((sector % 8) + 8) % 8;
        return RowForSector[sector];
    }
}
