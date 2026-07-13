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
    [SerializeField] private Button randomAllButton;

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

    private void Start() {
        for (var index = 0; index < choiceButtons.Length; index++) {
            var capturedIndex = index;
            choiceButtons[index].onClick.AddListener(delegate {
                OnChoiceClicked(capturedIndex);
            });
        }
        if (randomAllButton != null) {
            randomAllButton.onClick.AddListener(OnRandomAllClicked);
        }
        if (panel != null) {
            panel.SetActive(false);
        }
    }

    private void Update() {
        var gameManager = GameManager.Instance;
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

    private void ShowChoices() {
        isChoosing = true;
        Time.timeScale = 0f;
        if (panel != null) {
            panel.SetActive(true);
        }
        ShowStatStage();
    }

    private void ShowStatStage() {
        isWeaponStage = false;
        if (titleLabel != null) {
            titleLabel.text = "레벨 업! 능력치 선택";
        }
        for (var index = 0; index < currentChoices.Length; index++) {
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

    private void ShowWeaponStage() {
        isWeaponStage = true;
        if (titleLabel != null) {
            titleLabel.text = "레벨 업! 무기 선택";
        }
        RollChoices();
    }

    private void RollChoices() {
        var pool = new List<int>();
        for (var index = 0; index < WeaponDatabase.WeaponCount; index++) {
            pool.Add(index);
        }
        for (var index = 0; index < currentChoices.Length; index++) {
            var pickIndex = Random.Range(0, pool.Count);
            var weaponType = (WeaponType)pool[pickIndex];
            pool.RemoveAt(pickIndex);
            currentChoices[index] = weaponType;
            if (index < choiceIcons.Length && choiceIcons[index] != null) {
                choiceIcons[index].color = WeaponDatabase.GetIconColor(weaponType);
            }
            if (index < choiceIconChars.Length && choiceIconChars[index] != null) {
                choiceIconChars[index].text = WeaponDatabase.GetIconChar(weaponType);
            }
            if (index < choiceLabels.Length && choiceLabels[index] != null) {
                var ownedLevel = weaponManager != null ? weaponManager.GetWeaponLevel(weaponType) : 0;
                var suffix = ownedLevel > 0 ? " (Lv." + (ownedLevel + 1) + ")" : " (신규)";
                choiceLabels[index].text = WeaponDatabase.GetDisplayName(weaponType) + suffix;
            }
        }
    }

    private void OnChoiceClicked(int index) {
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

    private void OnRandomAllClicked() {
        while (GameManager.Instance != null && GameManager.Instance.HasPendingLevelUp()) {
            ApplyStatChoice(Random.Range(0, 3));
            if (weaponManager != null) {
                weaponManager.AddWeapon((WeaponType)Random.Range(0, WeaponDatabase.WeaponCount));
            }
            GameManager.Instance.ConsumeLevelUp();
        }
        if (panel != null) {
            panel.SetActive(false);
        }
        isChoosing = false;
        Time.timeScale = 1f;
    }

    private void ApplyStatChoice(int index) {
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
