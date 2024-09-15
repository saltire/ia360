using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoidsController))]
public class BoidsEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BoidsController boids = (BoidsController)target;

        if (GUILayout.Button("Add boid")) {
            boids.AddBoid();
        }

        if (GUILayout.Button("Add max boids")) {
            boids.AddMaxBoids();
        }

        if (GUILayout.Button("Clear boids")) {
            boids.ClearBoids();
        }
    }
}
