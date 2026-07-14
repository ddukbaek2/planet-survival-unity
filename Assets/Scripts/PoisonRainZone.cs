using UnityEngine;

public class PoisonRainZone : MonoBehaviour {
    private float radius = 3f;
    private float telegraphDuration = 0.9f;
    private float rainDuration = 2.6f;
    private float tickInterval = 0.18f;
    private int tickAttack = 30;
    private float dropletSpawnInterval = 0.05f;
    private float dropletHeight = 5f;

    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private Transform groundMarker;
    private Renderer markerRenderer;
    private float elapsedTime;
    private float tickTimer;
    private float dropletTimer;

    public void Configure(float zoneRadius, int poisonTickAttack) {
        radius = zoneRadius;
        tickAttack = poisonTickAttack;
    }

    private void Start() {
        var playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null) {
            playerTransform = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }
        BuildGroundMarker();
    }

    private void BuildGroundMarker() {
        var marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        marker.name = "PoisonMarker";
        var markerCollider = marker.GetComponent<Collider>();
        if (markerCollider != null) {
            Object.Destroy(markerCollider);
        }
        marker.transform.SetParent(transform, false);
        marker.transform.localPosition = new Vector3(0f, 0.02f, 0f);
        marker.transform.localScale = new Vector3(radius * 2f, 0.02f, radius * 2f);
        markerRenderer = marker.GetComponent<Renderer>();
        if (markerRenderer != null) {
            markerRenderer.material.SetColor("_BaseColor", new Color(0.32f, 0.85f, 0.16f));
            markerRenderer.material.EnableKeyword("_EMISSION");
            markerRenderer.material.SetColor("_EmissionColor", new Color(0.28f, 0.75f, 0.1f));
        }
        groundMarker = marker.transform;
    }

    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) {
            return;
        }
        elapsedTime += Time.deltaTime;
        if (elapsedTime < telegraphDuration) {
            var pulse = 0.75f + 0.25f * Mathf.Sin(elapsedTime * 20f);
            SetMarkerEmission(pulse);
            return;
        }
        if (elapsedTime < telegraphDuration + rainDuration) {
            SetMarkerEmission(1f);
            SpawnDroplets();
            ApplyPoisonTick();
            return;
        }
        Object.Destroy(gameObject);
    }

    private void SpawnDroplets() {
        dropletTimer -= Time.deltaTime;
        if (dropletTimer > 0f) {
            return;
        }
        dropletTimer = dropletSpawnInterval;
        for (var index = 0; index < 3; index++) {
            var droplet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            droplet.name = "PoisonDroplet";
            var dropletCollider = droplet.GetComponent<Collider>();
            if (dropletCollider != null) {
                Object.Destroy(dropletCollider);
            }
            var angle = Random.Range(0f, Mathf.PI * 2f);
            var distance = Random.Range(0f, radius);
            var offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
            var dropPosition = transform.position + offset;
            dropPosition.y = dropletHeight;
            droplet.transform.position = dropPosition;
            droplet.transform.localScale = new Vector3(0.18f, 0.3f, 0.18f);
            var dropletRenderer = droplet.GetComponent<Renderer>();
            if (dropletRenderer != null) {
                dropletRenderer.material.SetColor("_BaseColor", new Color(0.36f, 0.82f, 0.14f));
                dropletRenderer.material.EnableKeyword("_EMISSION");
                dropletRenderer.material.SetColor("_EmissionColor", new Color(0.3f, 0.78f, 0.1f));
            }
            var poisonDroplet = droplet.AddComponent<PoisonDroplet>();
            poisonDroplet.Configure(17f);
        }
    }

    private void ApplyPoisonTick() {
        if (playerTransform == null || playerHealth == null) {
            return;
        }
        var flatDelta = playerTransform.position - transform.position;
        flatDelta.y = 0f;
        if (flatDelta.magnitude > radius) {
            return;
        }
        tickTimer -= Time.deltaTime;
        if (tickTimer > 0f) {
            return;
        }
        tickTimer = tickInterval;
        playerHealth.ApplyHit(tickAttack);
    }

    private void SetMarkerEmission(float intensity) {
        if (markerRenderer == null) {
            return;
        }
        markerRenderer.material.SetColor("_EmissionColor", new Color(0.28f, 0.75f, 0.1f) * intensity);
    }
}
