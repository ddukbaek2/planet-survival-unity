using UnityEngine;
using TMPro;

public class BossBar : MonoBehaviour {
    public static BossBar Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform fill;
    [SerializeField] private TMP_Text nameLabel;

    private Enemy currentBoss;

    private void Awake() {
        Instance = this;
        if (panel != null) {
            panel.SetActive(false);
        }
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    public Transform GetCurrentBossTransform() {
        if (currentBoss == null) {
            return null;
        }
        return currentBoss.transform;
    }

    public void Show(Enemy boss, string bossName) {
        currentBoss = boss;
        if (nameLabel != null) {
            nameLabel.text = bossName;
        }
        if (panel != null) {
            panel.SetActive(true);
        }
        SetRatioInternal(1f);
    }

    public void SetRatio(Enemy boss, float ratio) {
        if (boss != currentBoss) {
            return;
        }
        SetRatioInternal(ratio);
    }

    public void Hide(Enemy boss) {
        if (boss != currentBoss) {
            return;
        }
        currentBoss = null;
        if (panel != null) {
            panel.SetActive(false);
        }
    }

    private void SetRatioInternal(float ratio) {
        if (fill != null) {
            var clampedRatio = Mathf.Clamp01(ratio);
            var fillScale = fill.localScale;
            fillScale.x = clampedRatio;
            fill.localScale = fillScale;
        }
    }
}
