using Jint;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;

public class CodeScript : MonoBehaviour {
    Engine engine;
    InGameTextEditor.TextEditor editor;
    BlocksScript blocks;
    Router router;
    PlanetsScript[] planetSystems;

    // public string script = @"
    //     const myVar = 108;
    //     log(`Hello from Javascript! ${myVar}`);
    // ";

    void Start() {
        engine = new Engine();
        editor = GetComponent<InGameTextEditor.TextEditor>();
        blocks = BlocksScript.instance;
        router = Router.instance;
        planetSystems = FindObjectsOfType<PlanetsScript>();

        engine.SetValue("log", new Action<string>(Log));

        engine.SetValue("load", new Action<string>(file => ExecuteFile("Scripts/" + file + ".js")));

        engine.SetValue("route", new Action<string, int, string>(router.SetRouteOutput));
        engine.SetValue("unroute", new Action<string, int>(router.ClearRouteOutput));
        engine.SetValue("map", new Action<string, int, float, float, float, float>(router.SetRouteMap));
        engine.SetValue("mapKeys", new Action<string, int>(router.SetRouteMapKeys));
        engine.SetValue("mapPads", new Action<string, int>(router.SetRouteMapPads));
        engine.SetValue("unmap", new Action<string, int>(router.ClearRouteMap));
        engine.SetValue("divide", new Action<string, int, int>(router.SetRouteDivide));
        engine.SetValue("delete", new Action<string, int>(router.DeleteRoute));

        engine.SetValue("set", new Action<string, float>(router.RouteValue));
        engine.SetValue("fade", new Action<string, float, float, float>(router.FadeValue));

        engine.SetValue("explode", new Action(blocks.ExplodeWalls));
        engine.SetValue("unexplode", new Action(blocks.UnexplodeWalls));

        engine.SetValue("shrink", new Action<int>(i => planetSystems[i - 1].ShrinkPlanets()));
        engine.SetValue("unshrink", new Action<int>(i => planetSystems[i - 1].UnshrinkPlanets()));

        ExecuteFile("Scripts/preload.js");
    }

    void ExecuteFile(string filename) {
        Debug.Log("Execute file: " + filename);
        ExecuteCode(File.ReadAllText(filename));
    }

    void ExecuteCode(string code) {
        try {
            engine.Execute(code);
        }
        catch (Exception e) {
            Debug.LogWarning(e);
        }
    }

    public void OnRunCode(InputValue inputValue) {
        if (inputValue.isPressed && editor.EditorActive) {
            string[] lines = editor.Text.Split("\n");
            string currentLine = lines[editor.CaretPosition.lineIndex];

            Debug.Log("Execute code: " + currentLine);
            ExecuteCode(currentLine);
        }
    }

    void Log(string msg) {
        Debug.Log("Log: " + msg);
    }
}
