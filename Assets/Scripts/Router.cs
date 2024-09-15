using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

class Route {
    public string output;
    public Queue<string> events;
    public float lastEventTime = 0;
    public (float minA, float maxA, float minB, float maxB) map;
    public int divide = 1;
    public int count = 0;
}

class Fade {
    public string output;
    public float startValue;
    public float endValue;
    public float duration;
    public float startTime;
}

public class Router : MonoBehaviour {
    public static Router instance;
    InGameTextEditor.TextEditor textEditor;
    LaserSpawner[] laserSpawners;
    PlanetsScript[] planetSystems;

    SortedDictionary<string, Route> routes = new SortedDictionary<string, Route>();
    List<Fade> fades = new List<Fade>();

    public int queueLength = 8;
    public float noteCooldown = .05f; // between 1/32 and 1/16 of a second

    bool textDirty = false;

    void Awake() {
        instance = this;
        textEditor = GetComponent<InGameTextEditor.TextEditor>();
        laserSpawners = FindObjectsOfType<LaserSpawner>();
        planetSystems = FindObjectsOfType<PlanetsScript>();
    }

    void UpdateText() {
        textDirty = true;
    }

    void Update() {
        if (textDirty) {
            textEditor.SetText(string.Join("\n", routes.Select(x => {
                Route r = x.Value;
                (float minA, float maxA, float minB, float maxB) = r.map;

                return x.Key + " "
                    + "▁▃▅▇"[r.count % 4] + " "
                    + "▁▃▅▇"[r.count / r.divide % 4] + " "
                    + string.Join(" ", r.events.ToArray())
                    + (r.divide > 1 ? string.Format(" {0}/{1}", r.count % r.divide, r.divide) : "")
                    + (minA != maxA ? string.Format(" ({0}-{1} > {2}-{3})", minA, maxA, minB, maxB) : "")
                    + (r.output != null ? " >> " + r.output : "");
            })));

            textDirty = false;
        }

        foreach (Fade fade in fades) {
            if (fade.startTime + fade.duration + .1f > Time.time) {
                RouteValue(fade.output, Mathf.Lerp(fade.startValue, fade.endValue,
                    (Time.time - fade.startTime) / fade.duration));
            }
        }
    }

    public string GetKey(string input, int channel) {
        if (input == "note") {
            return string.Format("Note {0,2}", channel);
        }

        if (input.ToLower().StartsWith("cc")) {
            return string.Format("CC{0,2} {1,2}",
                int.Parse(input.ToLower().Replace("cc", "")), channel);
        }

        throw new Exception("Invalid input");
    }

    Route GetRoute(string input, int channel) {
        string key = GetKey(input, channel);
        if (!routes.ContainsKey(key)) {
            routes[key] = new Route() {
                events = new Queue<string>()
            };
        }
        return routes[key];
    }

    public void ReceiveEvent(string input, int channel, string valueStr, float value, float value2) {
        Route route = GetRoute(input, channel);

        // Only allow one note per channel within a certain time period.
        // Prevents duplicates (and chords, but oh well).
        if (route.lastEventTime + noteCooldown > Time.time) return;

        Debug.Log(string.Format("{0}: {1}", GetKey(input, channel), value));

        route.events.Enqueue(valueStr);
        while (route.events.Count > queueLength) {
            route.events.Dequeue();
        }
        route.lastEventTime = Time.time;

        route.count = route.count + 1;
        int divide = route.divide > 1 ? route.divide : 1;

        if (route.output != null && route.count % divide == 0) {
            float mappedValue = value;
            (float minA, float maxA, float minB, float maxB) = route.map;
            if (minA != maxA) {
                mappedValue = Mathf.Lerp(minB, maxB, Mathf.InverseLerp(minA, maxA, value));
            }

            RouteValue(route.output, mappedValue);
        }

        UpdateText();
    }

    public void SetRouteOutput(string input, int channel, string output) {
        GetRoute(input, channel).output = output;
        UpdateText();
    }

    public void ClearRouteOutput(string input, int channel) {
        GetRoute(input, channel).output = null;
        UpdateText();
    }

    public void SetRouteMap(string input, int channel, float minA, float maxA, float minB, float maxB) {
        GetRoute(input, channel).map = (minA, maxA, minB, maxB);
        UpdateText();
    }

    public void SetRouteMapKeys(string input, int channel) {
        GetRoute(input, channel).map = (48, 72, 0, 1); // LaunchKey notes
        UpdateText();
    }

    public void SetRouteMapPads(string input, int channel) {
        GetRoute(input, channel).map = (36, 51, 0, 1); // LaunchKey pads
        UpdateText();
    }

    public void ClearRouteMap(string input, int channel) {
        GetRoute(input, channel).map = (0, 0, 0, 0);
        UpdateText();
    }

    public void SetRouteDivide(string input, int channel, int divide) {
        GetRoute(input, channel).divide = divide;
        UpdateText();
    }

    public void DeleteRoute(string input, int channel) {
        routes.Remove(GetKey(input, channel));
        UpdateText();
    }

    public void FadeValue(string output, float startValue, float endValue, float duration) {
        fades.Add(new Fade() {
            output = output,
            startValue = startValue,
            endValue = endValue,
            duration = duration,
            startTime = Time.time,
        });
    }

    public void RouteValue(string output, float value) {
        Color color = Color.HSVToRGB(value, 1, 1);

        // boids

        if (output == "spawnFly") {
            BoidsController.instance.AddBoid(color);
        }
        else if (output == "maxFlies") {
            BoidsController.instance.maxBoidCount = (int)value;
        }

        // blocks

        else if (output == "blockA") {
            BlocksScript.instance.ChangeBlockToA(color);
        }
        else if (output == "blockB") {
            BlocksScript.instance.ChangeBlockToB(color);
        }
        else if (output == "blockFlash") {
            BlocksScript.instance.FlashBlock(color);
        }

        // lasers

        else if (output == "spawnLaserAngle") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.SpawnLaser(value, null, null);
            }
        }
        else if (output == "spawnLaserColor") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.SpawnLaser(null, color, null);
            }
        }
        else if (output == "spawnLaserSpeed") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.SpawnLaser(null, null, value);
            }
        }
        else if (output == "laserAngle") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.laserAngle = value;
            }
        }
        else if (output == "laserColor") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.laserColor = color;
            }
        }
        else if (output == "laserSpeed") {
            foreach (LaserSpawner laserSpawner in laserSpawners) {
                laserSpawner.laserSpeed = value;
            }
        }

        // planets

        else if (output == "planets1Flash") {
            planetSystems[0].FlashPlanet(color);
        }
        else if (output == "planets2Flash") {
            planetSystems[1].FlashPlanet(color);
        }
        else if (output == "planets3Flash") {
            planetSystems[2].FlashPlanet(color);
        }

        // oscillating skybox

        else if (output == "cloudsColor") {
            RenderSettings.skybox.SetColor("_CloudsColor", color);
        }
        else if (output == "cloudsFadeIn") {
            RenderSettings.skybox.SetFloat("_CloudsFadeIn", value);
        }
        else if (output == "oscColor1") {
            RenderSettings.skybox.SetColor("_OscColor1", color);
        }
        else if (output == "oscColor2") {
            RenderSettings.skybox.SetColor("_OscColor2", color);
        }
        else if (output == "oscFreq") {
            RenderSettings.skybox.SetFloat("_OscFrequency", value);
        }
        else if (output == "oscSpeed") {
            RenderSettings.skybox.SetFloat("_OscSpeed", value);
        }
        else if (output == "oscTwist") {
            RenderSettings.skybox.SetFloat("_OscTwist", value);
        }
        else if (output == "oscFadeIn") {
            RenderSettings.skybox.SetFloat("_OscFadeIn", value);
        }
        else if (output == "cityFadeIn") {
            RenderSettings.skybox.SetFloat("_CityFadeIn", value);
        }
        else if (output == "cityColor") {
            RenderSettings.skybox.SetColor("_CityColor", color);
        }
        else if (output == "cityColorFadeIn") {
            RenderSettings.skybox.SetFloat("_CityColorFadeIn", value);
        }
        else if (output == "cityBlackOut") {
            RenderSettings.skybox.SetFloat("_CityBlackOut", value);
        }
    }
}
