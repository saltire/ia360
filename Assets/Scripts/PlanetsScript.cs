using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetsScript : MonoBehaviour {
    public PlanetScript planetPrefab;
    public float radius = 40;
    public int planetsCount = 12;
    public float rotateDuration = 20;
    public Color gizmoColor = Color.red;

    List<PlanetScript> planets;

    int flashIndex = 0;

    void Start() {
        SpawnPlanets();
    }

    void Update() {
        transform.Rotate(Vector3.up, 360 / rotateDuration * Time.deltaTime);
    }

    void OnDrawGizmos() {
        Gizmos.color = gizmoColor;

        for (int i = 0; i < planetsCount; i++) {
            Gizmos.DrawLine(transform.position,
                transform.position
                    + transform.rotation * (Quaternion.AngleAxis(360 / planetsCount * i, Vector3.up)
                    * Vector3.forward * radius));
        }
    }

    public void SpawnPlanets() {
        ClearPlanets();

        planets = new List<PlanetScript>();

        for (int i = 0; i < planetsCount; i++) {
            PlanetScript planet = Instantiate(planetPrefab,
                transform.position
                    + transform.rotation * (Quaternion.AngleAxis(360 / planetsCount * i, Vector3.up)
                    * Vector3.forward * radius),
                Quaternion.identity, transform);

            planets.Add(planet);
        }
    }

    public void ClearPlanets() {
        planets?.Clear();

        foreach (PlanetScript p in GetComponentsInChildren<PlanetScript>()) {
            DestroyImmediate(p.gameObject);
        }
    }

    public void FlashPlanet(Color color) {
        planets[flashIndex].Flash(color);
        flashIndex = (flashIndex + 1) % planets.Count;
    }

    public void ShrinkPlanets() {
        foreach (PlanetScript planet in planets) {
            planet.Shrink();
        }
    }

    public void UnshrinkPlanets() {
        foreach (PlanetScript planet in planets) {
            planet.Unshrink();
        }
    }
}
