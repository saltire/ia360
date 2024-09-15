using UnityEngine;

public class SunScript : MonoBehaviour {
    public float rotateTime = 19;

    public Color color1;
    public Color color2;
    public float colorTime = 11;

    Light lite;

    void Start() {
        lite = GetComponent<Light>();
    }

    void Update() {
        transform.Rotate(Quaternion.Inverse(transform.rotation) * Vector3.up, 360 / rotateTime * Time.deltaTime);

        lite.color = Color.Lerp(color1, color2, Mathf.Sin(Time.time * Mathf.PI * 2 / colorTime));
    }
}
