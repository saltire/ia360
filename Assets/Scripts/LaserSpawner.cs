using UnityEngine;

public class LaserSpawner : MonoBehaviour {
    public LaserScript laserPrefab;
    public float despawnDistance = 12;
    public bool reverseDirection;
    public float laserAngle = 0;
    public Color laserColor = Color.red;
    public float laserSpeed = 1;

    public void SpawnLaser(float? angle, Color? color, float? speed) {
        LaserScript laser = Instantiate(laserPrefab, transform.position, transform.rotation, transform);
        laser.reverse = reverseDirection;

        laser.transform.Rotate(reverseDirection ? Vector3.left : Vector3.right,
            angle is float angleV ? angleV : laserAngle);

        Color colorValue = color is Color colorV ? colorV : laserColor;
        laser.GetComponentInChildren<LineRenderer>().material.SetColor("_EmissionColor", colorValue);
        foreach (Light light in laser.GetComponentsInChildren<Light>()) {
            light.color = colorValue;
        }

        laser.speed = Mathf.Max(.1f, speed is float speedV ? speedV : laserSpeed);
    }

    void Update() {
        foreach (Transform laser in transform) {
            if (Vector3.Distance(laser.position, transform.position) > despawnDistance) {
                Destroy(laser.gameObject);
            }
        }
    }
}
