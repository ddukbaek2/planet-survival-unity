using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUI : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Image[] choiceIcons;
    [SerializeField] private TMP_Text[] choiceIconChars;
    [SerializeField] private TMP_Text[] choiceLabels;

    private readonly WeaponType[] currentChoices = new WeaponType[3];
    private bool isChoosing;

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
        if (weaponManager != null) {
            weaponManager.AddWeapon(currentChoices[index]);
        }
        if (GameManager.Instance != null) {
            GameManager.Instance.ConsumeLevelUp();
        }
        if (GameManager.Instance != null && GameManager.Instance.HasPendingLevelUp()) {
            RollChoices();
            return;
        }
        if (panel != null) {
            panel.SetActive(false);
        }
        isChoosing = false;
        Time.timeScale = 1f;
    }
}
