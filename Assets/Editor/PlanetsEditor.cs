using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetsScript))]
public class PlanetsEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PlanetsScript script = (PlanetsScript)target;

        if (GUILayout.Button("Spawn planets")) {
            script.SpawnPlanets();
        }

        if (GUILayout.Button("Clear planets")) {
            script.ClearPlanets();
        }

        if (GUILayout.Button("Flash planet")) {
            script.FlashPlanet(Color.red);
        }

        if (GUILayout.Button("Shrink planets")) {
            script.ShrinkPlanets();
        }

        if (GUILayout.Button("Unshrink planets")) {
            script.UnshrinkPlanets();
        }
    }
}
