using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlocksScript))]
public class BlocksEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BlocksScript script = (BlocksScript)target;

        if (GUILayout.Button("Build walls")) {
            script.BuildWalls();
        }

        if (GUILayout.Button("Explode walls")) {
            script.ExplodeWalls();
        }

        if (GUILayout.Button("Unexplode walls")) {
            script.UnexplodeWalls();
        }

        if (GUILayout.Button("Change block to material A")) {
            script.ChangeBlockToA(Color.green);
        }

        if (GUILayout.Button("Change block to material B")) {
            script.ChangeBlockToB(Color.blue);
        }

        if (GUILayout.Button("Change all blocks to material A")) {
            script.ChangeAllToA();
        }

        if (GUILayout.Button("Change all blocks to material B")) {
            script.ChangeAllToB();
        }

        if (GUILayout.Button("Flash block")) {
            script.FlashBlock(Color.red);
        }
    }
}
