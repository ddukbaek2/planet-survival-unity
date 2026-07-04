using UnityEngine;

public struct EnemyDefinition {
    public string displayName;
    public int health;
    public int attack;
    public int defense;
    public float moveSpeed;
    public float scale;

    public EnemyDefinition(string displayName, int health, int attack, int defense, float moveSpeed, float scale) {
        this.displayName = displayName;
        this.health = health;
        this.attack = attack;
        this.defense = defense;
        this.moveSpeed = moveSpeed;
        this.scale = scale;
    }
}

public static class EnemyTable {
    private static readonly EnemyDefinition[] Definitions = new EnemyDefinition[] {
        new EnemyDefinition("약졸", 2, 1, 0, 2.5f, 0.5f),
        new EnemyDefinition("돌격병", 2, 1, 0, 4.2f, 0.42f),
        new EnemyDefinition("방패병", 6, 2, 1, 1.8f, 0.72f),
        new EnemyDefinition("정예병", 10, 2, 2, 2.2f, 0.9f)
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
