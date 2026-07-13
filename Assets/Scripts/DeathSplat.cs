using UnityEngine;

public class DeathSplat : MonoBehaviour {
    [SerializeField] private string resourcePath = "Sprites/splat_green_sheet";
    [SerializeField] private float frameDuration = 0.045f;
    [SerializeField] private float lingerTime = 2.5f;

    private SpriteRenderer spriteRenderer;
    private readonly Sprite[] frames = new Sprite[8];
    private int frameCount;
    private float timer;
    private int currentFrame = -1;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sortingOrder = -1;
        }
        LoadFrames();
    }

    private void LoadFrames() {
        var loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        for (var index = 0; index < loadedSprites.Length; index++) {
            var spriteName = loadedSprites[index].name;
            var underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            var numberText = spriteName.Substring(underscoreIndex + 1);
            int frameIndex;
            if (int.TryParse(numberText, out frameIndex) && frameIndex >= 0 && frameIndex < frames.Length) {
                frames[frameIndex] = loadedSprites[index];
                if (frameIndex + 1 > frameCount) {
                    frameCount = frameIndex + 1;
                }
            }
        }
    }

    private void Update() {
        if (spriteRenderer == null || frameCount == 0) {
            Object.Destroy(gameObject);
            return;
        }
        timer += Time.deltaTime;
        var index = Mathf.FloorToInt(timer / frameDuration);
        if (index >= frameCount) {
            var overTime = timer - frameCount * frameDuration;
            if (overTime >= lingerTime) {
                Object.Destroy(gameObject);
                return;
            }
            SetFrame(frameCount - 1);
            var color = spriteRenderer.color;
            color.a = 1f - (overTime / lingerTime);
            spriteRenderer.color = color;
            return;
        }
        SetFrame(index);
    }

    private void SetFrame(int index) {
        if (index == currentFrame) {
            return;
        }
        currentFrame = index;
        if (frames[index] != null) {
            spriteRenderer.sprite = frames[index];
        }
    }
}
