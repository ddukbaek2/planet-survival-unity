using UnityEngine;

public struct EnemyDefinition {
    public string displayName;
    public int health;
    public int attack;
    public int defense;
    public float moveSpeed;
    public float scale;
    public string spriteResourcePath;
    public bool isBoss;
    public bool isElite;
    public bool spawnsWeb;

    public EnemyDefinition(string displayName, int health, int attack, int defense, float moveSpeed, float scale, string spriteResourcePath) {
        this.displayName = displayName;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.moveSpeed = moveSpeed;
        this.scale = scale;
        this.spriteResourcePath = spriteResourcePath;
        this.isBoss = false;
        this.isElite = false;
        this.spawnsWeb = false;
    }
}

public static class EnemyTable {
    private static readonly EnemyDefinition[] Definitions = new EnemyDefinition[] {
        new EnemyDefinition("약졸", 6, 1, 0, 2.5f, 0.5f, "Sprites/roach_sheet"),
        new EnemyDefinition("돌격병", 6, 1, 0, 4.2f, 0.42f, "Sprites/roach_sheet"),
        new EnemyDefinition("방패병", 18, 2, 1, 1.8f, 0.72f, "Sprites/roach_sheet"),
        new EnemyDefinition("정예병", 30, 2, 2, 2.2f, 0.9f, "Sprites/roach_sheet"),
        new EnemyDefinition("모기", 9, 1, 0, 5.0f, 0.5f, "Sprites/mosquito_sheet"),
        new EnemyDefinition("지네", 24, 2, 1, 2.0f, 0.85f, "Sprites/centipede_sheet"),
        new EnemyDefinition("맨티스", 18, 3, 1, 3.0f, 0.75f, "Sprites/mantis_sheet"),
        new EnemyDefinition("스파이더", 15, 2, 0, 3.8f, 0.7f, "Sprites/spider_sheet"),
        new EnemyDefinition("와스프", 12, 2, 0, 4.5f, 0.6f, "Sprites/wasp_sheet"),
        new EnemyDefinition("스콜피온", 42, 3, 3, 1.8f, 0.95f, "Sprites/scorpion_sheet")
    };

    static EnemyTable() {
        Definitions[7].spawnsWeb = true;
    }

    public static EnemyDefinition GetBoss() {
        EnemyDefinition boss = new EnemyDefinition("해충 군주", 50000, 30, 10, 1.6f, 2.8f, "Sprites/scorpion_sheet");
        boss.isBoss = true;
        return boss;
    }

    public static EnemyDefinition GetElite(int phase) {
        string[] sprites = { "Sprites/mantis_sheet", "Sprites/spider_sheet", "Sprites/wasp_sheet", "Sprites/scorpion_sheet", "Sprites/centipede_sheet" };
        int index = (phase - 1) % sprites.Length;
        int health = 20 + phase * 8;
        int attack = 2 + phase / 5;
        int defense = phase / 10;
        float scale = 1.1f + phase * 0.01f;
        EnemyDefinition elite = new EnemyDefinition("엘리트 " + phase, health, attack, defense, 2.4f, scale, sprites[index]);
        elite.isElite = true;
        if (sprites[index] == "Sprites/spider_sheet") {
            elite.spawnsWeb = true;
        }
        return elite;
    }

    public static EnemyDefinition GetMidBoss(int tier) {
        string[] names = { "지네 여왕", "사마귀 장군", "독거미 마님", "말벌 여왕" };
        string[] sprites = { "Sprites/centipede_sheet", "Sprites/mantis_sheet", "Sprites/spider_sheet", "Sprites/wasp_sheet" };
        int index = Mathf.Clamp(tier - 1, 0, 3);
        EnemyDefinition midBoss = new EnemyDefinition(names[index], 100 * tier, 3 + tier, tier, 2.2f, 1.4f + tier * 0.15f, sprites[index]);
        midBoss.isBoss = true;
        return midBoss;
    }

    public static int Count {
        get {
            return Definitions.Length;
        }
    }

    public static EnemyDefinition GetByIndex(int index) {
        return Definitions[index];
    }

    public static EnemyDefinition PickForTime(float elapsedSeconds) {
        int unlockedCount = 1 + Mathf.FloorToInt(elapsedSeconds / 20f);
        unlockedCount = Mathf.Clamp(unlockedCount, 1, Definitions.Length);
        int pickIndex = Random.Range(0, unlockedCount);
        return Definitions[pickIndex];
    }
}
