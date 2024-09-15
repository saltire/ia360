using UnityEngine;

public class LaserScript : MonoBehaviour {
    Quaternion currentRot = Quaternion.identity;

    public float speed;
    public bool reverse;

    void Update() {
        if (transform.rotation != currentRot) {
            // Stretch the laser out so its ends stay at the same Y position.
            transform.localScale = new Vector3(
                1, 1 / Mathf.Cos(Mathf.Clamp(transform.rotation.eulerAngles.x, -80, 80) * Mathf.Deg2Rad), 1);

            currentRot = transform.rotation;
        }

        transform.localPosition = transform.localPosition +
            (reverse ? Vector3.back : Vector3.forward) * speed * Time.deltaTime;
    }
}
