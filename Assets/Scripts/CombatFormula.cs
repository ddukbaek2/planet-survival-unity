using UnityEngine;

public static class CombatFormula {
    public static int ComputeDamage(int attackPower, int defense) {
        return Mathf.Max(1, attackPower - defense);
    }
}
