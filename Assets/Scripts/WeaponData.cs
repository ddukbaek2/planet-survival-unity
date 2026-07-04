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

public static class WeaponDatabase {
    public const int WeaponCount = 10;

    public static string GetDisplayName(WeaponType weaponType) {
        switch (weaponType) {
            case WeaponType.StraightShot: {
                return "직선탄";
            }
            case WeaponType.AimedShot: {
                return "조준탄";
            }
            case WeaponType.HomingMissile: {
                return "추적미사일";
            }
            case WeaponType.SpreadShot: {
                return "산탄";
            }
            case WeaponType.NovaBurst: {
                return "전방위탄";
            }
            case WeaponType.Boomerang: {
                return "부메랑";
            }
            case WeaponType.OrbitOrb: {
                return "궤도구체";
            }
            case WeaponType.SweepSlash: {
                return "휩쓸기";
            }
            case WeaponType.Mine: {
                return "지뢰";
            }
            case WeaponType.ExplosiveShot: {
                return "폭발탄";
            }
            default: {
                return "무기";
            }
        }
    }

    public static Color GetIconColor(WeaponType weaponType) {
        switch (weaponType) {
            case WeaponType.StraightShot: {
                return new Color(1f, 1f, 1f);
            }
            case WeaponType.AimedShot: {
                return new Color(0.5f, 0.9f, 1f);
            }
            case WeaponType.HomingMissile: {
                return new Color(1f, 0.55f, 0.45f);
            }
            case WeaponType.SpreadShot: {
                return new Color(1f, 0.85f, 0.35f);
            }
            case WeaponType.NovaBurst: {
                return new Color(0.72f, 0.62f, 1f);
            }
            case WeaponType.Boomerang: {
                return new Color(0.45f, 1f, 0.72f);
            }
            case WeaponType.OrbitOrb: {
                return new Color(0.6f, 0.82f, 1f);
            }
            case WeaponType.SweepSlash: {
                return new Color(1f, 0.62f, 0.9f);
            }
            case WeaponType.Mine: {
                return new Color(1f, 0.72f, 0.25f);
            }
            case WeaponType.ExplosiveShot: {
                return new Color(1f, 0.42f, 0.32f);
            }
            default: {
                return Color.white;
            }
        }
    }
}
