using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockScript))]
public class BlockEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BlockScript script = (BlockScript)target;

        if (GUILayout.Button("Flash")) {
          script.Flash(Color.red);
        }
    }
}
