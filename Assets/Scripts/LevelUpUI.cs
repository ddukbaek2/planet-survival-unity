using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Image[] choiceIcons;
    [SerializeField] private TMP_Text[] choiceIconChars;
    [SerializeField] private TMP_Text[] choiceLabels;

    private readonly WeaponType[] currentChoices = new WeaponType[3];
    private bool isChoosing;
    private bool isWeaponStage;

    private static readonly string[] StatLabels = { "체력 +10", "공격력 +1", "방어력 +1" };
    private static readonly string[] StatIconChars = { "체", "공", "방" };
    private static readonly Color[] StatIconColors = {
        new Color(0.5f, 1f, 0.5f),
        new Color(1f, 0.5f, 0.5f),
        new Color(0.5f, 0.7f, 1f)
    };

    void Start() {
        for (int index = 0; index < choiceButtons.Length; index++) {
            int capturedIndex = index;
            choiceButtons[index].onClick.AddListener(delegate {
                OnChoiceClicked(capturedIndex);
            });
        }
        if (panel != null) {
            panel.SetActive(false);
        }
    }

    void Update() {
        GameManager gameManager = GameManager.Instance;
        if (gameManager == null) {
            return;
        }
        if (gameManager.IsGameOver()) {
            return;
        }
        if (!isChoosing && gameManager.HasPendingLevelUp()) {
            ShowChoices();
        }
    }

    void ShowChoices() {
        isChoosing = true;
        Time.timeScale = 0f;
        if (panel != null) {
            panel.SetActive(true);
        }
        ShowStatStage();
    }

    void ShowStatStage() {
        isWeaponStage = false;
        if (titleLabel != null) {
            titleLabel.text = "레벨 업! 능력치 선택";
        }
        for (int index = 0; index < currentChoices.Length; index++) {
            if (index < choiceIcons.Length && choiceIcons[index] != null) {
                choiceIcons[index].color = StatIconColors[index];
            }
            if (index < choiceIconChars.Length && choiceIconChars[index] != null) {
                choiceIconChars[index].text = StatIconChars[index];
            }
            if (index < choiceLabels.Length && choiceLabels[index] != null) {
                choiceLabels[index].text = StatLabels[index];
            }
        }
    }

    void ShowWeaponStage() {
        isWeaponStage = true;
        if (titleLabel != null) {
            titleLabel.text = "레벨 업! 무기 선택";
        }
        RollChoices();
    }

    void RollChoices() {
        List<int> pool = new List<int>();
        for (int index = 0; index < WeaponDatabase.WeaponCount; index++) {
            pool.Add(index);
        }
        for (int index = 0; index < currentChoices.Length; index++) {
            int pickIndex = Random.Range(0, pool.Count);
            WeaponType weaponType = (WeaponType)pool[pickIndex];
            pool.RemoveAt(pickIndex);
            currentChoices[index] = weaponType;
            if (index < choiceIcons.Length && choiceIcons[index] != null) {
                choiceIcons[index].color = WeaponDatabase.GetIconColor(weaponType);
            }
            if (index < choiceIconChars.Length && choiceIconChars[index] != null) {
                choiceIconChars[index].text = WeaponDatabase.GetIconChar(weaponType);
            }
            if (index < choiceLabels.Length && choiceLabels[index] != null) {
                int ownedLevel = weaponManager != null ? weaponManager.GetWeaponLevel(weaponType) : 0;
                string suffix = ownedLevel > 0 ? " (Lv." + (ownedLevel + 1) + ")" : " (신규)";
                choiceLabels[index].text = WeaponDatabase.GetDisplayName(weaponType) + suffix;
            }
        }
    }

    void OnChoiceClicked(int index) {
        if (!isWeaponStage) {
            ApplyStatChoice(index);
            ShowWeaponStage();
            return;
        }
        if (weaponManager != null) {
            weaponManager.AddWeapon(currentChoices[index]);
        }
        if (GameManager.Instance != null) {
            GameManager.Instance.ConsumeLevelUp();
        }
        if (GameManager.Instance != null && GameManager.Instance.HasPendingLevelUp()) {
            ShowStatStage();
            return;
        }
        if (panel != null) {
            panel.SetActive(false);
        }
        isChoosing = false;
        Time.timeScale = 1f;
    }

    void ApplyStatChoice(int index) {
        if (playerHealth == null) {
            return;
        }
        switch (index) {
            case 0: {
                playerHealth.AddMaxHealth(10);
                break;
            }
            case 1: {
                playerHealth.AddAttackPower(1);
                break;
            }
            case 2: {
                playerHealth.AddDefense(1);
                break;
            }
            default: {
                break;
            }
        }
    }
}
