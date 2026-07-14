using UnityEngine;

public class PlayerSprite : MonoBehaviour {
    private enum PlayerState {
        Idle,
        Move,
        Attack,
        Death
    }

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private string resourcePath = "Sprites/ext_topdown_sheet";
    [SerializeField] private float moveThreshold = 0.02f;
    [SerializeField] private float attackRange = 12f;
    [SerializeField] private float idleFrameDuration = 0.28f;
    [SerializeField] private float moveFrameDuration = 0.11f;
    [SerializeField] private float attackFrameDuration = 0.08f;
    [SerializeField] private float deathFrameDuration = 0.14f;

    private static readonly int[] idleFrames = { 0, 1 };
    private static readonly int[] moveFrames = { 4, 5, 6, 7 };
    private static readonly int[] attackFrames = { 8, 9, 10, 11 };
    private static readonly int[] deathFrames = { 12, 13, 14, 15 };

    private static readonly Quaternion flatRotation = Quaternion.Euler(90f, 0f, 0f);

    private Transform anchorTransform;
    private Vector3 lastPosition;
    private float currentYaw;
    private Sprite[] frameSprites;
    private PlayerState currentState;
    private int frameCursor;
    private float frameTimer;
    private float scanTimer;
    private bool enemyNear;
    private bool isMoving;
    private bool deathFinished;

    private void Awake() {
        anchorTransform = transform.parent;
        if (anchorTransform != null) {
            lastPosition = anchorTransform.position;
        }
        LoadFrames();
        currentState = PlayerState.Idle;
        frameCursor = 0;
        ApplyCurrentFrame();
    }

    private void LoadFrames() {
        frameSprites = new Sprite[16];
        if (spriteRenderer != null) {
            spriteRenderer.flipX = false;
        }
        var loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (var index = 0; index < loadedSprites.Length; index++) {
            var spriteName = loadedSprites[index].name;
            var underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            var numberText = spriteName.Substring(underscoreIndex + 1);
            int frameIndex;
            if (int.TryParse(numberText, out frameIndex) && frameIndex >= 0 && frameIndex < frameSprites.Length) {
                frameSprites[frameIndex] = loadedSprites[index];
            }
        }
    }

    private void Update() {
        UpdateProximityScan();
        var nextState = ResolveState();
        if (nextState != currentState) {
            currentState = nextState;
            frameCursor = 0;
            frameTimer = 0f;
            if (currentState == PlayerState.Death) {
                deathFinished = false;
            }
            ApplyCurrentFrame();
        }
        Animate();
    }

    private void UpdateProximityScan() {
        scanTimer -= Time.unscaledDeltaTime;
        if (scanTimer > 0f) {
            return;
        }
        scanTimer = 0.25f;
        enemyNear = false;
        if (anchorTransform == null) {
            return;
        }
        var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        var rangeSquared = attackRange * attackRange;
        var playerPosition = anchorTransform.position;
        for (var index = 0; index < enemies.Length; index++) {
            var distanceSquared = (enemies[index].transform.position - playerPosition).sqrMagnitude;
            if (distanceSquared <= rangeSquared) {
                enemyNear = true;
                return;
            }
        }
    }

    private PlayerState ResolveState() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return PlayerState.Death;
        }
        if (isMoving) {
            return PlayerState.Move;
        }
        if (enemyNear) {
            return PlayerState.Attack;
        }
        return PlayerState.Idle;
    }

    private void Animate() {
        var frames = GetCurrentFrames();
        if (frames.Length == 0) {
            return;
        }
        if (currentState == PlayerState.Death && deathFinished) {
            return;
        }
        frameTimer += Time.unscaledDeltaTime;
        var frameDuration = GetCurrentFrameDuration();
        if (frameTimer < frameDuration) {
            return;
        }
        frameTimer -= frameDuration;
        frameCursor++;
        if (frameCursor >= frames.Length) {
            if (currentState == PlayerState.Death) {
                frameCursor = frames.Length - 1;
                deathFinished = true;
            }
            else {
                frameCursor = 0;
            }
        }
        ApplyCurrentFrame();
    }

    private void ApplyCurrentFrame() {
        if (spriteRenderer == null || frameSprites == null) {
            return;
        }
        var frames = GetCurrentFrames();
        if (frames.Length == 0) {
            return;
        }
        var clampedCursor = Mathf.Clamp(frameCursor, 0, frames.Length - 1);
        var frameIndex = frames[clampedCursor];
        var frameSprite = frameSprites[frameIndex];
        if (frameSprite != null) {
            spriteRenderer.sprite = frameSprite;
        }
    }

    private int[] GetCurrentFrames() {
        if (currentState == PlayerState.Move) {
            return moveFrames;
        }
        if (currentState == PlayerState.Attack) {
            return attackFrames;
        }
        if (currentState == PlayerState.Death) {
            return deathFrames;
        }
        return idleFrames;
    }

    private float GetCurrentFrameDuration() {
        if (currentState == PlayerState.Move) {
            return moveFrameDuration;
        }
        if (currentState == PlayerState.Attack) {
            return attackFrameDuration;
        }
        if (currentState == PlayerState.Death) {
            return deathFrameDuration;
        }
        return idleFrameDuration;
    }

    private void LateUpdate() {
        if (anchorTransform == null || spriteRenderer == null) {
            return;
        }
        var currentPosition = anchorTransform.position;
        var moveDelta = currentPosition - lastPosition;
        moveDelta.y = 0f;
        lastPosition = currentPosition;
        isMoving = moveDelta.magnitude > moveThreshold;
        if (isMoving) {
            currentYaw = Mathf.Atan2(moveDelta.x, moveDelta.z) * Mathf.Rad2Deg;
        }
        transform.rotation = Quaternion.AngleAxis(currentYaw, Vector3.up) * flatRotation;
    }
}
