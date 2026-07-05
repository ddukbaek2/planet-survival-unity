using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject orbitPrefab;
    [SerializeField] private GameObject sweepPrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private float targetingRange = 22f;

    private readonly List<WeaponType> ownedWeapons = new List<WeaponType>();
    private readonly Dictionary<WeaponType, int> weaponLevels = new Dictionary<WeaponType, int>();
    private readonly Dictionary<WeaponType, float> cooldownTimers = new Dictionary<WeaponType, float>();
    private PlayerHealth playerHealth;

    void Start() {
        playerHealth = GetComponent<PlayerHealth>();
        AddWeapon(WeaponType.StraightShot);
    }

    public void AddWeapon(WeaponType weaponType) {
        if (!weaponLevels.ContainsKey(weaponType)) {
            weaponLevels[weaponType] = 0;
            cooldownTimers[weaponType] = 0f;
            ownedWeapons.Add(weaponType);
        }
        weaponLevels[weaponType] = weaponLevels[weaponType] + 1;
    }

    public bool HasWeapon(WeaponType weaponType) {
        return weaponLevels.ContainsKey(weaponType);
    }

    public List<WeaponType> GetOwnedWeapons() {
        return ownedWeapons;
    }

    public int GetWeaponLevel(WeaponType weaponType) {
        int level;
        if (weaponLevels.TryGetValue(weaponType, out level)) {
            return level;
        }
        return 0;
    }

    void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        for (int index = 0; index < ownedWeapons.Count; index++) {
            WeaponType weaponType = ownedWeapons[index];
            cooldownTimers[weaponType] = cooldownTimers[weaponType] - Time.deltaTime;
            if (cooldownTimers[weaponType] > 0f) {
                continue;
            }
            cooldownTimers[weaponType] = GetCooldown(weaponType);
            int weaponLevel = weaponLevels[weaponType];
            FireWeapon(weaponType, weaponLevel);
        }
    }

    float GetCooldown(WeaponType weaponType) {
        float baseCooldown = WeaponDatabase.GetCooldown(weaponType);
        int weaponLevel = GetWeaponLevel(weaponType);
        float scaledCooldown = baseCooldown * Mathf.Pow(0.9f, Mathf.Max(0, weaponLevel - 1));
        return Mathf.Max(0.1f, scaledCooldown);
    }

    int GetAttackPower(WeaponType weaponType) {
        int playerAttack = playerHealth != null ? playerHealth.GetAttackPower() : 3;
        float multiplier = WeaponDatabase.GetDamageMultiplier(weaponType);
        return Mathf.Max(1, Mathf.RoundToInt(playerAttack * multiplier));
    }

    void FireWeapon(WeaponType weaponType, int weaponLevel) {
        int attackPower = GetAttackPower(weaponType);
        switch (weaponType) {
            case WeaponType.StraightShot: {
                Vector3 direction = GetForwardDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                break;
            }
            case WeaponType.AimedShot: {
                Vector3 direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                break;
            }
            case WeaponType.HomingMissile: {
                Vector3 direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Homing, 0, 0f);
                break;
            }
            case WeaponType.SpreadShot: {
                Vector3 direction = GetNearestDirection();
                int pelletCount = 3 + (weaponLevel - 1);
                float spreadDegrees = 30f;
                for (int index = 0; index < pelletCount; index++) {
                    float ratio = pelletCount == 1 ? 0f : ((float)index / (pelletCount - 1) - 0.5f);
                    float angle = ratio * spreadDegrees;
                    Vector3 pelletDirection = Quaternion.Euler(0f, angle, 0f) * direction;
                    SpawnProjectile(pelletDirection, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                }
                break;
            }
            case WeaponType.NovaBurst: {
                int novaCount = 8 + (weaponLevel - 1) * 2;
                for (int index = 0; index < novaCount; index++) {
                    float angle = 360f * index / novaCount;
                    Vector3 novaDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                    SpawnProjectile(novaDirection, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                }
                break;
            }
            case WeaponType.Boomerang: {
                Vector3 direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Boomerang, 999, 0f);
                break;
            }
            case WeaponType.OrbitOrb: {
                int orbCount = 1 + weaponLevel;
                for (int index = 0; index < orbCount; index++) {
                    float startAngle = 360f * index / orbCount;
                    SpawnOrbit(startAngle, attackPower);
                }
                break;
            }
            case WeaponType.SweepSlash: {
                SpawnSweep(weaponLevel, attackPower);
                break;
            }
            case WeaponType.Mine: {
                SpawnMine(attackPower);
                break;
            }
            case WeaponType.ExplosiveShot: {
                Vector3 direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 2.2f);
                break;
            }
            default: {
                break;
            }
        }
    }

    Vector3 GetForwardDirection() {
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        if (forwardDirection.sqrMagnitude < 0.001f) {
            return Vector3.forward;
        }
        return forwardDirection.normalized;
    }

    Vector3 GetNearestDirection() {
        Enemy nearestEnemy = GetNearestEnemy();
        if (nearestEnemy == null) {
            return GetForwardDirection();
        }
        Vector3 direction = nearestEnemy.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) {
            return GetForwardDirection();
        }
        return direction.normalized;
    }

    Enemy GetNearestEnemy() {
        Enemy[] enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        float nearestDistance = targetingRange * targetingRange;
        Vector3 currentPosition = transform.position;
        for (int index = 0; index < enemies.Length; index++) {
            float distance = (enemies[index].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestEnemy = enemies[index];
            }
        }
        return nearestEnemy;
    }

    void SpawnProjectile(Vector3 direction, int attackPower, Projectile.ProjectileMode projectileMode, int pierce, float explosion) {
        Quaternion spawnRotation = Quaternion.LookRotation(direction);
        Projectile projectile;
        if (ProjectilePool.Instance != null) {
            projectile = ProjectilePool.Instance.Get(transform.position, spawnRotation);
        }
        else {
            if (projectilePrefab == null) {
                return;
            }
            GameObject projectileObject = Object.Instantiate(projectilePrefab, transform.position, spawnRotation);
            projectile = projectileObject.GetComponent<Projectile>();
        }
        if (projectile != null) {
            projectile.Configure(direction, attackPower, 9f, 3f, projectileMode, pierce, explosion);
        }
    }

    void SpawnOrbit(float startAngle, int attackPower) {
        if (orbitPrefab == null) {
            return;
        }
        GameObject orbitObject = Object.Instantiate(orbitPrefab, transform.position, Quaternion.identity);
        OrbitProjectile orbit = orbitObject.GetComponent<OrbitProjectile>();
        if (orbit != null) {
            orbit.Configure(transform, 2.6f, 200f, attackPower, 4f, startAngle);
        }
    }

    void SpawnSweep(int weaponLevel, int attackPower) {
        if (sweepPrefab == null) {
            return;
        }
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0.06f;
        GameObject sweepObject = Object.Instantiate(sweepPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        SweepAttack sweep = sweepObject.GetComponent<SweepAttack>();
        if (sweep != null) {
            float sweepRadius = 5f + weaponLevel;
            sweep.Configure(attackPower, sweepRadius, 0.3f);
        }
    }

    void SpawnMine(int attackPower) {
        if (minePrefab == null) {
            return;
        }
        Vector3 spawnPosition = transform.position;
        spawnPosition.y = 0.05f;
        GameObject mineObject = Object.Instantiate(minePrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        MineField mine = mineObject.GetComponent<MineField>();
        if (mine != null) {
            mine.Configure(attackPower, 2.6f, 4f, 0.5f);
        }
    }
}
