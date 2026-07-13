using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageAnnouncer : MonoBehaviour {
    public static StageAnnouncer Instance { get; private set; }

    private const float fadeInDuration = 0.24f;
    private const float fadeOutDuration = 0.55f;
    private const float displayDuration = 1.9f;

    private CanvasGroup canvasGroup;
    private RectTransform rootRect;
    private TMP_Text titleLabel;
    private TMP_Text subtitleLabel;

    private float elapsedTime;
    private bool isPlaying;

    public static void Show(string title, string subtitle, Color accentColor) {
        EnsureInstance();
        Instance.Play(title, subtitle, accentColor);
    }

    private static void EnsureInstance() {
        if (Instance != null) {
            return;
        }
        var announcerObject = new GameObject("StageAnnouncer");
        announcerObject.AddComponent<StageAnnouncer>();
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        BuildUserInterface();
        canvasGroup.alpha = 0f;
        isPlaying = false;
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void BuildUserInterface() {
        var canvasObject = new GameObject("AnnouncerCanvas");
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32000;
        var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.matchWidthOrHeight = 0.5f;

        var rootObject = new GameObject("Root");
        rootObject.transform.SetParent(canvasObject.transform, false);
        rootRect = rootObject.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);
        rootRect.anchoredPosition = new Vector2(0f, 160f);
        rootRect.sizeDelta = new Vector2(1920f, 320f);
        canvasGroup = rootObject.AddComponent<CanvasGroup>();

        var barObject = new GameObject("Bar");
        barObject.transform.SetParent(rootObject.transform, false);
        var barImage = barObject.AddComponent<Image>();
        barImage.color = new Color(0f, 0f, 0f, 0.42f);
        var barRect = barImage.rectTransform;
        barRect.anchorMin = new Vector2(0.5f, 0.5f);
        barRect.anchorMax = new Vector2(0.5f, 0.5f);
        barRect.pivot = new Vector2(0.5f, 0.5f);
        barRect.sizeDelta = new Vector2(1920f, 250f);
        barRect.anchoredPosition = Vector2.zero;

        var koreanFont = Resources.Load<TMP_FontAsset>("Fonts/경기천년바탕_Regular SDF");

        var titleObject = new GameObject("Title");
        titleObject.transform.SetParent(rootObject.transform, false);
        titleLabel = titleObject.AddComponent<TextMeshProUGUI>();
        if (koreanFont != null) {
            titleLabel.font = koreanFont;
        }
        titleLabel.fontSize = 128f;
        titleLabel.fontStyle = FontStyles.Bold;
        titleLabel.alignment = TextAlignmentOptions.Center;
        titleLabel.enableWordWrapping = false;
        var titleRect = titleLabel.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 0.5f);
        titleRect.anchorMax = new Vector2(0.5f, 0.5f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(1920f, 180f);
        titleRect.anchoredPosition = new Vector2(0f, 42f);

        var subtitleObject = new GameObject("Subtitle");
        subtitleObject.transform.SetParent(rootObject.transform, false);
        subtitleLabel = subtitleObject.AddComponent<TextMeshProUGUI>();
        if (koreanFont != null) {
            subtitleLabel.font = koreanFont;
        }
        subtitleLabel.fontSize = 54f;
        subtitleLabel.fontStyle = FontStyles.Bold;
        subtitleLabel.alignment = TextAlignmentOptions.Center;
        subtitleLabel.enableWordWrapping = false;
        subtitleLabel.color = Color.white;
        var subtitleRect = subtitleLabel.rectTransform;
        subtitleRect.anchorMin = new Vector2(0.5f, 0.5f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.5f);
        subtitleRect.pivot = new Vector2(0.5f, 0.5f);
        subtitleRect.sizeDelta = new Vector2(1920f, 90f);
        subtitleRect.anchoredPosition = new Vector2(0f, -68f);
    }

    private void Play(string title, string subtitle, Color accentColor) {
        titleLabel.text = title;
        titleLabel.color = accentColor;
        subtitleLabel.text = subtitle;
        elapsedTime = 0f;
        isPlaying = true;
    }

    private void Update() {
        if (!isPlaying) {
            return;
        }
        elapsedTime += Time.unscaledDeltaTime;
        var alpha = 1f;
        var scale = 1f;
        if (elapsedTime < fadeInDuration) {
            var progress = elapsedTime / fadeInDuration;
            alpha = progress;
            scale = Mathf.Lerp(1.25f, 1f, progress);
        }
        else if (elapsedTime > displayDuration - fadeOutDuration) {
            var progress = (elapsedTime - (displayDuration - fadeOutDuration)) / fadeOutDuration;
            alpha = 1f - progress;
            scale = Mathf.Lerp(1f, 1.06f, progress);
        }
        canvasGroup.alpha = alpha;
        rootRect.localScale = new Vector3(scale, scale, 1f);
        if (elapsedTime >= displayDuration) {
            isPlaying = false;
            canvasGroup.alpha = 0f;
        }
    }
}
