using System.Collections.Generic;
using UnityEngine;

public class BoidsController : MonoBehaviour {
    public static BoidsController instance;

    public int maxBoidCount = 100;
    public Boid boidPrefab;

    public float boidSpeed = 3;
    public float boidPerceptionRadius = 1;
    public Vector3 cageSize = new Vector3(5, 5, 5);
    public float avoidWallsTurnDist = .75f;
    public float avoidCenterTurnDist = 1.5f;

    public float separationWeight = 25;
    public float cohesionWeight = 20;
    public float alignmentWeight = 10;
    public float avoidWallsWeight = 30;
    public float avoidCenterWeight = 30;

    public Queue<Boid> boids = new Queue<Boid>();
    public bool fillOnStart = true;

    void Awake() {
        instance = this;
    }

    void Start() {
        foreach (Boid boid in GetComponentsInChildren<Boid>()) {
            boids.Enqueue(boid);
        }

        CullQueue();

        if (fillOnStart) {
            AddMaxBoids();
        }
    }

    void Update() {
        CullQueue();
    }

    public void AddBoid() {
        AddBoid(null);
    }

    public void AddBoid(Color? color) {
        Vector3 pos = new Vector3(
            Random.Range(-cageSize.x / 2f, cageSize.x / 2f),
            Random.Range(-cageSize.y / 2f, cageSize.y / 2f),
            Random.Range(-cageSize.z / 2f, cageSize.z / 2f)
        );
        Quaternion rot = Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        );

        Boid newBoid = Instantiate(boidPrefab, pos, rot, transform);
        boids.Enqueue(newBoid);

        if (color is Color colorValue) {
            newBoid.GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", colorValue);
            newBoid.GetComponentInChildren<Light>().color = colorValue;
        }

        CullQueue();
    }

    public void AddMaxBoids() {
        CullQueue();

        while (boids.Count < maxBoidCount) {
            AddBoid();
        }
    }

    public void ClearBoids() {
        boids.Clear();

        foreach (Boid boid in GetComponentsInChildren<Boid>()) {
            DestroyImmediate(boid.gameObject);
        }
    }

    void CullQueue() {
        while (boids.Count > maxBoidCount) {
            Boid boid = boids.Dequeue();

            if (boid) {
                DestroyImmediate(boid.gameObject);
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, cageSize);
    }
}
