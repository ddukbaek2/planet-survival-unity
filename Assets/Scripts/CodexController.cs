using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CodexController : MonoBehaviour {
    [SerializeField] private string titleSceneName = "Title";

    private Image enemyImage;
    private TMP_Text nameLabel;
    private TMP_Text typeLabel;
    private TMP_Text statsLabel;
    private TMP_Text skillsLabel;
    private TMP_Text pageLabel;
    private TMP_FontAsset koreanFont;
    private List<EnemyDefinition> entries;
    private int currentIndex;

    private void Start() {
        BuildUserInterface();
        entries = EnemyTable.GetCodexEntries();
        currentIndex = 0;
        RefreshPage();
    }

    private void BuildUserInterface() {
        koreanFont = Resources.Load<TMP_FontAsset>("Fonts/경기천년바탕_Regular SDF");

        var canvasObject = new GameObject("CodexCanvas");
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        var background = CreateImage("Background", canvasObject.transform, new Color(0.06f, 0.07f, 0.09f, 1f));
        StretchFull(background.rectTransform);

        var titleText = CreateText("Title", canvasObject.transform, "적 도감", 72f, FontStyles.Bold, TextAlignmentOptions.Center, new Color(0.85f, 0.95f, 0.7f));
        SetRect(titleText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -80f), new Vector2(900f, 100f));

        var backButton = CreateButton("Back", canvasObject.transform, new Vector2(0f, 1f), "뒤로", new Vector2(160f, -70f), new Vector2(220f, 84f), new Color(0.4f, 0.2f, 0.22f, 1f));
        backButton.onClick.AddListener(GoBack);

        var imageFrame = CreateImage("ImageFrame", canvasObject.transform, new Color(0.1f, 0.12f, 0.16f, 1f));
        SetRect(imageFrame.rectTransform, new Vector2(0.5f, 1f), new Vector2(-470f, -400f), new Vector2(440f, 440f));

        enemyImage = CreateImage("EnemyImage", imageFrame.transform, Color.white);
        enemyImage.preserveAspect = true;
        SetRect(enemyImage.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(400f, 400f));

        nameLabel = CreateText("Name", canvasObject.transform, "", 62f, FontStyles.Bold, TextAlignmentOptions.Left, Color.white);
        SetRect(nameLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(340f, -215f), new Vector2(920f, 84f));

        typeLabel = CreateText("Type", canvasObject.transform, "", 38f, FontStyles.Bold, TextAlignmentOptions.Left, new Color(0.9f, 0.75f, 0.35f));
        SetRect(typeLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(340f, -290f), new Vector2(920f, 52f));

        statsLabel = CreateText("Stats", canvasObject.transform, "", 40f, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Color(0.85f, 0.9f, 0.95f));
        SetRect(statsLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(340f, -485f), new Vector2(920f, 300f));

        skillsLabel = CreateText("Skills", canvasObject.transform, "", 34f, FontStyles.Normal, TextAlignmentOptions.TopLeft, new Color(0.7f, 0.95f, 0.75f));
        SetRect(skillsLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -800f), new Vector2(1500f, 240f));
        skillsLabel.enableWordWrapping = true;

        var previousButton = CreateButton("Prev", canvasObject.transform, new Vector2(0.5f, 0f), "이전", new Vector2(-300f, 74f), new Vector2(230f, 84f), new Color(0.2f, 0.24f, 0.3f, 1f));
        previousButton.onClick.AddListener(ShowPrevious);

        pageLabel = CreateText("Page", canvasObject.transform, "", 40f, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        SetRect(pageLabel.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 74f), new Vector2(280f, 84f));

        var nextButton = CreateButton("Next", canvasObject.transform, new Vector2(0.5f, 0f), "다음", new Vector2(300f, 74f), new Vector2(230f, 84f), new Color(0.2f, 0.24f, 0.3f, 1f));
        nextButton.onClick.AddListener(ShowNext);
    }

    private void RefreshPage() {
        if (entries == null || entries.Count == 0) {
            return;
        }
        currentIndex = Mathf.Clamp(currentIndex, 0, entries.Count - 1);
        var entry = entries[currentIndex];
        var enemySprite = LoadEnemySprite(entry.spriteResourcePath);
        if (enemyImage != null) {
            enemyImage.sprite = enemySprite;
            enemyImage.enabled = enemySprite != null;
        }
        if (nameLabel != null) {
            nameLabel.text = entry.displayName;
        }
        if (typeLabel != null) {
            typeLabel.text = GetTypeText(entry);
        }
        if (statsLabel != null) {
            statsLabel.text = GetStatsText(entry);
        }
        if (skillsLabel != null) {
            skillsLabel.text = "[스킬]\n" + GetSkillsText(entry);
        }
        if (pageLabel != null) {
            pageLabel.text = (currentIndex + 1) + " / " + entries.Count;
        }
    }

    private void ShowPrevious() {
        currentIndex -= 1;
        if (currentIndex < 0) {
            currentIndex = entries.Count - 1;
        }
        RefreshPage();
    }

    private void ShowNext() {
        currentIndex += 1;
        if (currentIndex >= entries.Count) {
            currentIndex = 0;
        }
        RefreshPage();
    }

    private void GoBack() {
        SceneManager.LoadScene(titleSceneName);
    }

    private string GetTypeText(EnemyDefinition definition) {
        if (definition.isBoss) {
            return "보스";
        }
        if (definition.isElite) {
            return "엘리트";
        }
        return "일반";
    }

    private string GetStatsText(EnemyDefinition definition) {
        var statsText = "체력      " + definition.health;
        statsText += "\n공격      " + definition.attack;
        statsText += "\n방어      " + definition.defense;
        statsText += "\n이동 속도   " + definition.moveSpeed.ToString("0.0");
        statsText += "\n크기      " + definition.scale.ToString("0.00");
        return statsText;
    }

    private string GetSkillsText(EnemyDefinition definition) {
        if (definition.isBoss) {
            return "- 돌진 : 짧게 가속해 플레이어에게 돌격\n- 충격파 : 주변에 광역 피해\n- 독침 미사일 : 유도 발사체 3발\n- 독액 비 : 장판에 독이 쏟아져 지속 피해";
        }
        if (definition.spawnsWeb) {
            return "- 거미줄 : 이동 속도를 늦추는 지대를 주기적으로 생성";
        }
        return "특수 스킬 없음 (접촉 시 피해)";
    }

    private Sprite LoadEnemySprite(string resourcePath) {
        var loadedSprites = Resources.LoadAll<Sprite>(resourcePath);
        if (loadedSprites == null || loadedSprites.Length == 0) {
            return null;
        }
        var fallbackSprite = loadedSprites[0];
        for (var index = 0; index < loadedSprites.Length; index++) {
            var spriteName = loadedSprites[index].name;
            var underscoreIndex = spriteName.LastIndexOf('_');
            if (underscoreIndex < 0) {
                continue;
            }
            int frameIndex;
            if (int.TryParse(spriteName.Substring(underscoreIndex + 1), out frameIndex) && frameIndex == 16) {
                return loadedSprites[index];
            }
        }
        return fallbackSprite;
    }

    private Image CreateImage(string objectName, Transform parent, Color color) {
        var imageObject = new GameObject(objectName);
        imageObject.transform.SetParent(parent, false);
        var image = imageObject.AddComponent<Image>();
        image.color = color;
        return image;
    }

    private TMP_Text CreateText(string objectName, Transform parent, string content, float fontSize, FontStyles fontStyle, TextAlignmentOptions alignment, Color color) {
        var textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);
        var text = textObject.AddComponent<TextMeshProUGUI>();
        if (koreanFont != null) {
            text.font = koreanFont;
        }
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = color;
        text.enableWordWrapping = false;
        return text;
    }

    private Button CreateButton(string objectName, Transform parent, Vector2 anchor, string label, Vector2 anchoredPosition, Vector2 size, Color backgroundColor) {
        var buttonObject = new GameObject(objectName);
        buttonObject.transform.SetParent(parent, false);
        var buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = backgroundColor;
        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        SetRect(buttonImage.rectTransform, anchor, anchoredPosition, size);
        var buttonLabel = CreateText("Label", buttonObject.transform, label, 36f, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
        StretchFull(buttonLabel.rectTransform);
        return button;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchor, Vector2 anchoredPosition, Vector2 size) {
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchoredPosition;
    }

    private void StretchFull(RectTransform rectTransform) {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
