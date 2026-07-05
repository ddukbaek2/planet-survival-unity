using UnityEngine;

public struct EnemyDefinition {
    public string displayName;
    public int health;
    public int attack;
    public int defense;
    public float moveSpeed;
    public float scale;
    public string spriteResourcePath;

    public EnemyDefinition(string displayName, int health, int attack, int defense, float moveSpeed, float scale, string spriteResourcePath) {
        this.displayName = displayName;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.moveSpeed = moveSpeed;
        this.scale = scale;
        this.spriteResourcePath = spriteResourcePath;
    }
}

public static class EnemyTable {
    private static readonly EnemyDefinition[] Definitions = new EnemyDefinition[] {
        new EnemyDefinition("약졸", 2, 1, 0, 2.5f, 0.5f, "Sprites/roach_sheet"),
        new EnemyDefinition("돌격병", 2, 1, 0, 4.2f, 0.42f, "Sprites/roach_sheet"),
        new EnemyDefinition("방패병", 6, 2, 1, 1.8f, 0.72f, "Sprites/roach_sheet"),
        new EnemyDefinition("정예병", 10, 2, 2, 2.2f, 0.9f, "Sprites/roach_sheet"),
        new EnemyDefinition("모기", 3, 1, 0, 5.0f, 0.5f, "Sprites/mosquito_sheet"),
        new EnemyDefinition("지네", 8, 2, 1, 2.0f, 0.85f, "Sprites/centipede_sheet"),
        new EnemyDefinition("맨티스", 6, 3, 1, 3.0f, 0.75f, "Sprites/mantis_sheet"),
        new EnemyDefinition("스파이더", 5, 2, 0, 3.8f, 0.7f, "Sprites/spider_sheet"),
        new EnemyDefinition("와스프", 4, 2, 0, 4.5f, 0.6f, "Sprites/wasp_sheet"),
        new EnemyDefinition("스콜피온", 14, 3, 3, 1.8f, 0.95f, "Sprites/scorpion_sheet")
    };

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
