using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIconBar : MonoBehaviour {
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private float iconSize = 44f;
    [SerializeField] private float spacing = 8f;

    private int lastCount = -1;
    private readonly List<WeaponType> iconWeapons = new List<WeaponType>();
    private readonly List<TMP_Text> levelLabels = new List<TMP_Text>();

    void Update() {
        if (weaponManager == null) {
            return;
        }
        List<WeaponType> ownedWeapons = weaponManager.GetOwnedWeapons();
        if (ownedWeapons.Count != lastCount) {
            Rebuild(ownedWeapons);
            lastCount = ownedWeapons.Count;
        }
        for (int index = 0; index < iconWeapons.Count; index++) {
            int weaponLevel = weaponManager.GetWeaponLevel(iconWeapons[index]);
            levelLabels[index].text = "Lv" + weaponLevel;
        }
    }

    void Rebuild(List<WeaponType> ownedWeapons) {
        for (int index = transform.childCount - 1; index >= 0; index--) {
            Object.Destroy(transform.GetChild(index).gameObject);
        }
        iconWeapons.Clear();
        levelLabels.Clear();
        for (int index = 0; index < ownedWeapons.Count; index++) {
            WeaponType weaponType = ownedWeapons[index];
            GameObject iconObject = new GameObject("Buff_" + weaponType, typeof(Image));
            iconObject.transform.SetParent(transform, false);
            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = iconSprite;
            iconImage.color = WeaponDatabase.GetIconColor(weaponType);
            iconImage.raycastTarget = false;
            RectTransform iconRect = iconObject.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 1f);
            iconRect.anchorMax = new Vector2(0f, 1f);
            iconRect.pivot = new Vector2(0f, 1f);
            iconRect.sizeDelta = new Vector2(iconSize, iconSize);
            iconRect.anchoredPosition = new Vector2(index * (iconSize + spacing), 0f);

            GameObject charObject = new GameObject("Char", typeof(TextMeshProUGUI));
            charObject.transform.SetParent(iconObject.transform, false);
            TextMeshProUGUI charLabel = charObject.GetComponent<TextMeshProUGUI>();
            charLabel.font = fontAsset;
            charLabel.text = WeaponDatabase.GetIconChar(weaponType);
            charLabel.fontSize = 24f;
            charLabel.alignment = TextAlignmentOptions.Center;
            charLabel.color = Color.black;
            charLabel.raycastTarget = false;
            RectTransform charRect = charObject.GetComponent<RectTransform>();
            charRect.anchorMin = Vector2.zero;
            charRect.anchorMax = Vector2.one;
            charRect.offsetMin = Vector2.zero;
            charRect.offsetMax = Vector2.zero;

            GameObject levelObject = new GameObject("Level", typeof(TextMeshProUGUI));
            levelObject.transform.SetParent(iconObject.transform, false);
            TextMeshProUGUI levelLabel = levelObject.GetComponent<TextMeshProUGUI>();
            levelLabel.font = fontAsset;
            levelLabel.fontSize = 12f;
            levelLabel.alignment = TextAlignmentOptions.BottomRight;
            levelLabel.color = new Color(0.1f, 0.1f, 0.1f);
            levelLabel.raycastTarget = false;
            RectTransform levelRect = levelObject.GetComponent<RectTransform>();
            levelRect.anchorMin = Vector2.zero;
            levelRect.anchorMax = Vector2.one;
            levelRect.offsetMin = new Vector2(2f, 1f);
            levelRect.offsetMax = new Vector2(-2f, -1f);

            iconWeapons.Add(weaponType);
            levelLabels.Add(levelLabel);
        }
    }
}
