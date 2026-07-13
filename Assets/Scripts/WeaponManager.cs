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

    private void Start() {
        playerHealth = GetComponent<PlayerHealth>();
        AddWeapon(WeaponType.StraightShot);
    }

    public void AddWeapon(WeaponType weaponType) {
        if (!weaponLevels.ContainsKey(weaponType)) {
            weaponLevels[weaponType] = 0;
            cooldownTimers[weaponType] = 0f;
            ownedWeapons.Add(weaponType);
        }
        if (weaponLevels[weaponType] < 10) {
            weaponLevels[weaponType] = weaponLevels[weaponType] + 1;
        }
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

    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        for (var index = 0; index < ownedWeapons.Count; index++) {
            var weaponType = ownedWeapons[index];
            cooldownTimers[weaponType] = cooldownTimers[weaponType] - Time.deltaTime;
            if (cooldownTimers[weaponType] > 0f) {
                continue;
            }
            cooldownTimers[weaponType] = GetCooldown(weaponType);
            var weaponLevel = weaponLevels[weaponType];
            FireWeapon(weaponType, weaponLevel);
        }
    }

    private float GetCooldown(WeaponType weaponType) {
        var baseCooldown = WeaponDatabase.GetCooldown(weaponType);
        var weaponLevel = GetWeaponLevel(weaponType);
        var levelSteps = Mathf.Max(0, weaponLevel - 1);
        var perLevelFactor = 0.9f;
        if (weaponType == WeaponType.Mine) {
            perLevelFactor = 0.8f;
        }
        else if (weaponType == WeaponType.StraightShot) {
            perLevelFactor = 0.85f;
        }
        var scaledCooldown = baseCooldown * Mathf.Pow(perLevelFactor, levelSteps);
        return Mathf.Max(0.1f, scaledCooldown);
    }

    private int GetAttackPower(WeaponType weaponType) {
        var playerAttack = playerHealth != null ? playerHealth.GetAttackPower() : 3;
        var multiplier = WeaponDatabase.GetDamageMultiplier(weaponType);
        return Mathf.Max(1, Mathf.RoundToInt(playerAttack * multiplier));
    }

    private void FireWeapon(WeaponType weaponType, int weaponLevel) {
        var attackPower = GetAttackPower(weaponType);
        switch (weaponType) {
            case WeaponType.StraightShot: {
                var direction = GetForwardDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                break;
            }
            case WeaponType.AimedShot: {
                var direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                break;
            }
            case WeaponType.HomingMissile: {
                var direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Homing, 0, 0f);
                break;
            }
            case WeaponType.SpreadShot: {
                var direction = GetNearestDirection();
                var pelletCount = 3 + (weaponLevel - 1);
                var spreadDegrees = 30f;
                for (var index = 0; index < pelletCount; index++) {
                    var ratio = pelletCount == 1 ? 0f : ((float)index / (pelletCount - 1) - 0.5f);
                    var angle = ratio * spreadDegrees;
                    var pelletDirection = Quaternion.Euler(0f, angle, 0f) * direction;
                    SpawnProjectile(pelletDirection, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                }
                break;
            }
            case WeaponType.NovaBurst: {
                var novaCount = 8 + (weaponLevel - 1) * 2;
                for (var index = 0; index < novaCount; index++) {
                    var angle = 360f * index / novaCount;
                    var novaDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
                    SpawnProjectile(novaDirection, attackPower, Projectile.ProjectileMode.Straight, 0, 0f);
                }
                break;
            }
            case WeaponType.Boomerang: {
                var direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Boomerang, 999, 0f);
                break;
            }
            case WeaponType.OrbitOrb: {
                var orbCount = weaponLevel;
                for (var index = 0; index < orbCount; index++) {
                    var startAngle = 360f * index / orbCount;
                    SpawnOrbit(startAngle, attackPower);
                }
                break;
            }
            case WeaponType.SweepSlash: {
                SpawnSweep(weaponLevel, attackPower);
                break;
            }
            case WeaponType.Mine: {
                SpawnMine(weaponLevel, attackPower);
                break;
            }
            case WeaponType.ExplosiveShot: {
                var direction = GetNearestDirection();
                SpawnProjectile(direction, attackPower, Projectile.ProjectileMode.Straight, 0, 2.2f);
                break;
            }
            default: {
                break;
            }
        }
    }

    private Vector3 GetForwardDirection() {
        var forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        if (forwardDirection.sqrMagnitude < 0.001f) {
            return Vector3.forward;
        }
        return forwardDirection.normalized;
    }

    private Vector3 GetNearestDirection() {
        var nearestEnemy = GetNearestEnemy();
        if (nearestEnemy == null) {
            return GetForwardDirection();
        }
        var direction = nearestEnemy.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) {
            return GetForwardDirection();
        }
        return direction.normalized;
    }

    private Enemy GetNearestEnemy() {
        var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy nearestEnemy = null;
        var nearestDistance = targetingRange * targetingRange;
        var currentPosition = transform.position;
        for (var index = 0; index < enemies.Length; index++) {
            var distance = (enemies[index].transform.position - currentPosition).sqrMagnitude;
            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestEnemy = enemies[index];
            }
        }
        return nearestEnemy;
    }

    private void SpawnProjectile(Vector3 direction, int attackPower, Projectile.ProjectileMode projectileMode, int pierce, float explosion) {
        var spawnRotation = Quaternion.LookRotation(direction);
        Projectile projectile;
        if (ProjectilePool.Instance != null) {
            projectile = ProjectilePool.Instance.Get(transform.position, spawnRotation);
        }
        else {
            if (projectilePrefab == null) {
                return;
            }
            var projectileObject = Object.Instantiate(projectilePrefab, transform.position, spawnRotation);
            projectile = projectileObject.GetComponent<Projectile>();
        }
        if (projectile != null) {
            projectile.Configure(direction, attackPower, 9f, 3f, projectileMode, pierce, explosion);
        }
    }

    private void SpawnOrbit(float startAngle, int attackPower) {
        if (orbitPrefab == null) {
            return;
        }
        var orbitObject = Object.Instantiate(orbitPrefab, transform.position, Quaternion.identity);
        var orbit = orbitObject.GetComponent<OrbitProjectile>();
        if (orbit != null) {
            orbit.Configure(transform, 2.6f, 100f, attackPower, 4f, startAngle);
        }
    }

    private void SpawnSweep(int weaponLevel, int attackPower) {
        if (sweepPrefab == null) {
            return;
        }
        var spawnPosition = transform.position;
        spawnPosition.y = 0.06f;
        var sweepObject = Object.Instantiate(sweepPrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        var sweep = sweepObject.GetComponent<SweepAttack>();
        if (sweep != null) {
            var sweepRadius = 5f + weaponLevel;
            sweep.Configure(attackPower, sweepRadius, 0.3f);
        }
    }

    private void SpawnMine(int weaponLevel, int attackPower) {
        if (minePrefab == null) {
            return;
        }
        var spawnPosition = transform.position;
        spawnPosition.y = 0.05f;
        var mineObject = Object.Instantiate(minePrefab, spawnPosition, Quaternion.Euler(90f, 0f, 0f));
        var mine = mineObject.GetComponent<MineField>();
        if (mine != null) {
            var mineRadius = 1.3f * Mathf.Pow(1.2f, Mathf.Max(0, weaponLevel - 1));
            mine.Configure(attackPower, mineRadius, 4f, 0.5f);
        }
    }
}
