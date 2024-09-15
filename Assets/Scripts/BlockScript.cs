using UnityEngine;

public class BlockScript : MonoBehaviour {
    Vector3 startPosition;
    Vector3 targetPosition;
    Quaternion targetRotation;
    Vector3 startScale;

    public float explodeDistance = 20;
    public float explodeTime = 5;
    float explodeTimeElapsed;
    bool exploding;
    bool unexploding;

    MeshRenderer mesh;
    public float flashFadeTime = 1;
    Color flashColor;
    float flashTime;

    void Start() {
        startPosition = transform.position;
        targetPosition = startPosition.normalized * explodeDistance;
        targetRotation = Quaternion.Euler(
            Random.Range(0f, 720f),
            Random.Range(0f, 720f),
            Random.Range(0f, 720f)
        );
        startScale = transform.localScale;

        mesh = GetComponent<MeshRenderer>();
    }

    float Ease(float x) {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }

    void Update() {
        if (exploding && explodeTimeElapsed < explodeTime) {
            float phase = Ease(explodeTimeElapsed / explodeTime);

            transform.position = Vector3.Lerp(startPosition, targetPosition, phase);
            transform.rotation = Quaternion.Lerp(Quaternion.identity, targetRotation, phase);
            transform.localScale = startScale * Mathf.InverseLerp(1, .75f, phase);

            explodeTimeElapsed += Time.deltaTime;
        }
        else if (unexploding && explodeTimeElapsed < explodeTime) {
            float phase = Ease(explodeTimeElapsed / explodeTime);

            transform.position = Vector3.Lerp(targetPosition, startPosition, phase);
            transform.rotation = Quaternion.Lerp(targetRotation, Quaternion.identity, phase);
            transform.localScale = startScale * Mathf.InverseLerp(0, .25f, phase);

            explodeTimeElapsed += Time.deltaTime;
        }

        if (flashTime + flashFadeTime + .1f > Time.time) {
            mesh.material.SetColor("_EmissionColor", Color.Lerp(flashColor, Color.black,
                (Time.time - flashTime) / flashFadeTime));
        }
    }

    public void Flash(Color color) {
        mesh.material.SetColor("_EmissionColor", color);
        flashColor = color;
        flashTime = Time.time;
    }

    public void Explode() {
        exploding = true;
        unexploding = false;
        explodeTimeElapsed = 0;
    }

    public void Unexplode() {
        unexploding = true;
        exploding = false;
        explodeTimeElapsed = 0;
    }
}
