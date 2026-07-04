using UnityEngine;

public class GroundFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private float textureTilingPerUnit = 0.04f;

    private Material groundMaterial;

    void Awake() {
        Renderer groundRenderer = GetComponent<Renderer>();
        groundMaterial = groundRenderer.material;
    }

    void LateUpdate() {
        if (target == null) {
            return;
        }
        Vector3 targetPosition = target.position;
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector2 textureOffset = new Vector2(targetPosition.x, targetPosition.z) * textureTilingPerUnit;
        groundMaterial.SetTextureOffset("_BaseMap", textureOffset);
    }
}
