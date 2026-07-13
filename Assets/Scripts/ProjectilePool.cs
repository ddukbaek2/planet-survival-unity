using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    private static ProjectilePool instance;

    [SerializeField] private GameObject projectilePrefab;

    private readonly Stack<Projectile> inactiveProjectiles = new Stack<Projectile>();
    private int totalCount;
    private int activeCount;

    public static ProjectilePool Instance {
        get {
            return instance;
        }
    }

    private void Awake() {
        instance = this;
    }

    private void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }

    public int GetActiveCount() {
        return activeCount;
    }

    public int GetTotalCount() {
        return totalCount;
    }

    public Projectile Get(Vector3 position, Quaternion rotation) {
        Projectile projectile;
        if (inactiveProjectiles.Count > 0) {
            projectile = inactiveProjectiles.Pop();
        }
        else {
            var projectileObject = Object.Instantiate(projectilePrefab);
            projectile = projectileObject.GetComponent<Projectile>();
            projectile.SetPool(this);
            totalCount += 1;
        }
        projectile.transform.SetPositionAndRotation(position, rotation);
        projectile.gameObject.SetActive(true);
        activeCount += 1;
        return projectile;
    }

    public void Release(Projectile projectile) {
        projectile.gameObject.SetActive(false);
        inactiveProjectiles.Push(projectile);
        activeCount -= 1;
        if (activeCount < 0) {
            activeCount = 0;
        }
    }
}
