using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LaserSpawner))]
public class LasersEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        LaserSpawner lasers = (LaserSpawner)target;

        if (GUILayout.Button("Spawn laser")) {
            lasers.SpawnLaser(null, null, null);
        }
    }
}
