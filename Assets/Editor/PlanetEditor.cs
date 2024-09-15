using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetScript))]
public class PlanetEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        PlanetScript script = (PlanetScript)target;

        if (GUILayout.Button("Flash")) {
          script.Flash(Color.red);
        }
    }
}
