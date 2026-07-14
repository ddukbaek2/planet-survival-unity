using UnityEngine;

public class PoisonDroplet : MonoBehaviour {
    private float fallSpeed = 17f;
    private float groundY = 0.08f;

    public void Configure(float speed) {
        fallSpeed = speed;
    }

    private void Update() {
        var position = transform.position;
        position.y -= fallSpeed * Time.deltaTime;
        if (position.y <= groundY) {
            Object.Destroy(gameObject);
            return;
        }
        transform.position = position;
    }
}
