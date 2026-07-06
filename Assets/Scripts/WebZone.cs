using UnityEngine;

public class WebZone : MonoBehaviour {
    [SerializeField] private string resourcePath = "Sprites/web_white_sheet";
    [SerializeField] private float growDuration = 0.4f;
    [SerializeField] private float lifeTime = 8f;

    private SpriteRenderer spriteRenderer;
    private readonly Sprite[] frames = new Sprite[8];
    private int frameCount;
    private float timer;
    private int currentFrame = -1;
    private bool playerInside;
    private PlayerController affectedPlayer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sortingOrder = -1;
        }
        LoadFrames();
    }

    void LoadFrames() {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (int index = 0; index < loadedSprites.Length; index++) {
            string spriteName = loadedSprites[index].name;
            int underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            int frameIndex;
            if (int.TryParse(spriteName.Substring(underscoreIndex + 1), out frameIndex) && frameIndex >= 0 && frameIndex < frames.Length) {
                frames[frameIndex] = loadedSprites[index];
                if (frameIndex + 1 > frameCount) {
                    frameCount = frameIndex + 1;
                }
            }
        }
    }

    void Update() {
        timer += Time.deltaTime;
        if (frameCount > 0 && spriteRenderer != null) {
            float growStep = growDuration / frameCount;
            int index = Mathf.Min(frameCount - 1, Mathf.FloorToInt(timer / Mathf.Max(0.0001f, growStep)));
            if (index != currentFrame && frames[index] != null) {
                currentFrame = index;
                spriteRenderer.sprite = frames[index];
            }
        }
        if (timer >= lifeTime) {
            Object.Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null) {
            playerInside = true;
            affectedPlayer = playerController;
            playerController.AddSlow();
        }
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) {
            return;
        }
        Object.Destroy(gameObject);
    }

    void OnDestroy() {
        if (playerInside && affectedPlayer != null) {
            affectedPlayer.RemoveSlow();
            playerInside = false;
        }
    }
}
