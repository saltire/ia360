using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetScript : MonoBehaviour {
    MeshRenderer mesh;
    public float flashFadeTime = 1;
    Color flashColor;
    float flashTime;

    Vector3 startScale;
    public float flashSize = 2;

    bool shrinking;
    bool unshrinking;
    public float shrinkTime = 5;
    float shrinkTimeElapsed;

    Vector3 rotateAxis;
    public float rotateDuration = 8;

    void Awake() {
        mesh = GetComponentInChildren<MeshRenderer>();
        startScale = transform.localScale;

        rotateAxis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    float Ease(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    void Update() {
        if (flashTime + flashFadeTime + .1f > Time.time) {
            float flashPhase = (Time.time - flashTime) / flashFadeTime;

            mesh.material.SetColor("_EmissionColor", Color.Lerp(flashColor, Color.black, flashPhase));

            transform.localScale = Vector3.Lerp(startScale * flashSize, startScale, flashPhase);
        }

        if (shrinking && shrinkTimeElapsed < shrinkTime) {
            float phase = Ease(shrinkTimeElapsed / shrinkTime);

            transform.localScale = startScale * (1 - phase);

            shrinkTimeElapsed += Time.deltaTime;
        }

        if (unshrinking && shrinkTimeElapsed < shrinkTime) {
            float phase = Ease(shrinkTimeElapsed / shrinkTime);

            transform.localScale = startScale * phase;

            shrinkTimeElapsed += Time.deltaTime;
        }

        transform.Rotate(rotateAxis, 360 / rotateDuration * Time.deltaTime);
    }

    public void Flash(Color color) {
        flashTime = Time.time;

        mesh.material.SetColor("_EmissionColor", color);
        flashColor = color;

        transform.localScale = startScale * flashSize;
    }

    public void Shrink() {
        shrinking = true;
        unshrinking = false;
        shrinkTimeElapsed = 0;
    }

    public void Unshrink() {
        shrinking = false;
        unshrinking = true;
        shrinkTimeElapsed = 0;
    }
}
