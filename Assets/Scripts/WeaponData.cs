using UnityEngine;

public enum WeaponType {
    StraightShot,
    AimedShot,
    HomingMissile,
    SpreadShot,
    NovaBurst,
    Boomerang,
    OrbitOrb,
    SweepSlash,
    Mine,
    ExplosiveShot
}

public struct WeaponDefinition {
    public WeaponType type;
    public string displayName;
    public string iconChar;
    public Color iconColor;
    public float cooldown;
    public float damageMultiplier;

    public WeaponDefinition(WeaponType type, string displayName, string iconChar, Color iconColor, float cooldown, float damageMultiplier) {
        this.type = type;
        this.displayName = displayName;
        this.iconChar = iconChar;
        this.iconColor = iconColor;
        this.cooldown = cooldown;
        this.damageMultiplier = damageMultiplier;
    }
}

public static class WeaponDatabase {
    public const int WeaponCount = 10;

    private static readonly WeaponDefinition[] Definitions = new WeaponDefinition[] {
        new WeaponDefinition(WeaponType.StraightShot, "직선탄", "직", new Color(1f, 1f, 1f), 0.5f, 1f),
        new WeaponDefinition(WeaponType.AimedShot, "조준탄", "조", new Color(0.5f, 0.9f, 1f), 0.5f, 1f),
        new WeaponDefinition(WeaponType.HomingMissile, "추적미사일", "추", new Color(1f, 0.55f, 0.45f), 0.9f, 1.2f),
        new WeaponDefinition(WeaponType.SpreadShot, "산탄", "산", new Color(1f, 0.85f, 0.35f), 0.8f, 0.8f),
        new WeaponDefinition(WeaponType.NovaBurst, "전방위탄", "전", new Color(0.72f, 0.62f, 1f), 1.2f, 0.8f),
        new WeaponDefinition(WeaponType.Boomerang, "부메랑", "부", new Color(0.45f, 1f, 0.72f), 1.0f, 1f),
        new WeaponDefinition(WeaponType.OrbitOrb, "궤도구체", "궤", new Color(0.6f, 0.82f, 1f), 3.0f, 0.5f),
        new WeaponDefinition(WeaponType.SweepSlash, "휩쓸기", "휩", new Color(1f, 0.62f, 0.9f), 5.0f, 0.3f),
        new WeaponDefinition(WeaponType.Mine, "지뢰", "지", new Color(1f, 0.72f, 0.25f), 5.0f, 1.2f),
        new WeaponDefinition(WeaponType.ExplosiveShot, "폭발탄", "폭", new Color(1f, 0.42f, 0.32f), 1.1f, 1.5f)
    };

    public static WeaponDefinition Get(WeaponType weaponType) {
        for (int index = 0; index < Definitions.Length; index++) {
            if (Definitions[index].type == weaponType) {
                return Definitions[index];
            }
        }
        return Definitions[0];
    }

    public static WeaponDefinition GetByIndex(int index) {
        return Definitions[index];
    }

    public static string GetDisplayName(WeaponType weaponType) {
        return Get(weaponType).displayName;
    }

    public static string GetIconChar(WeaponType weaponType) {
        return Get(weaponType).iconChar;
    }

    public static Color GetIconColor(WeaponType weaponType) {
        return Get(weaponType).iconColor;
    }

    public static float GetCooldown(WeaponType weaponType) {
        return Get(weaponType).cooldown;
    }

    public static float GetDamageMultiplier(WeaponType weaponType) {
        return Get(weaponType).damageMultiplier;
    }
}
