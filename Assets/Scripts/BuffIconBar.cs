using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIconBar : MonoBehaviour {
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private Sprite iconSprite;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private float iconSize = 40f;
    [SerializeField] private float spacing = 8f;

    private int lastCount = -1;
    private readonly List<WeaponType> iconWeapons = new List<WeaponType>();
    private readonly List<TMP_Text> iconLabels = new List<TMP_Text>();

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
            iconLabels[index].text = weaponLevel.ToString();
        }
    }

    void Rebuild(List<WeaponType> ownedWeapons) {
        for (int index = transform.childCount - 1; index >= 0; index--) {
            Object.Destroy(transform.GetChild(index).gameObject);
        }
        iconWeapons.Clear();
        iconLabels.Clear();
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

            GameObject labelObject = new GameObject("Level", typeof(TextMeshProUGUI));
            labelObject.transform.SetParent(iconObject.transform, false);
            TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
            label.font = fontAsset;
            label.fontSize = 18f;
            label.alignment = TextAlignmentOptions.BottomRight;
            label.color = Color.black;
            label.raycastTarget = false;
            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(2f, 2f);
            labelRect.offsetMax = new Vector2(-3f, -2f);
            iconWeapons.Add(weaponType);
            iconLabels.Add(label);
        }
    }
}
