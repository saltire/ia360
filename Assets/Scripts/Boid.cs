using UnityEngine;

public class Boid : MonoBehaviour {
    private BoidsController controller;

    private Vector3 separationForce;
    private Vector3 cohesionForce;
    private Vector3 alignmentForce;
    private Vector3 avoidWallsForce;
    private Vector3 avoidCenterForce;

    private void Start() {
        controller = BoidsController.instance;
    }

    private void Update() {
        Vector3 seperationSum = Vector3.zero;
        Vector3 positionSum = Vector3.zero;
        Vector3 headingSum = Vector3.zero;

        int boidsNearby = 0;

        foreach (Boid boid in controller.boids) {
            if (boid != this) {
                Transform other = boid.transform;
                float distToOtherBoid = (transform.position - other.position).magnitude;

                if (distToOtherBoid < controller.boidPerceptionRadius) {
                    seperationSum += (transform.position - other.position)
                        * (1f / Mathf.Max(distToOtherBoid, .0001f));
                    positionSum += other.position;
                    headingSum += other.forward;

                    boidsNearby++;
                }
            }
        }

        if (boidsNearby > 0) {
            separationForce = seperationSum / boidsNearby;
            cohesionForce = (positionSum / boidsNearby) - transform.position;
            alignmentForce = headingSum / boidsNearby;
        }
        else {
            separationForce = Vector3.zero;
            cohesionForce = Vector3.zero;
            alignmentForce = Vector3.zero;
        }

        float minDistToBorder = Mathf.Min(
            controller.cageSize.x / 2f - Mathf.Abs(transform.position.x),
            controller.cageSize.y / 2f - Mathf.Abs(transform.position.y),
            controller.cageSize.z / 2f - Mathf.Abs(transform.position.z)
        );
        avoidWallsForce = minDistToBorder < controller.avoidWallsTurnDist
            ? -transform.position.normalized
            : Vector3.zero;

        Vector2 flatPos = new Vector2(transform.position.x, transform.position.z);
        avoidCenterForce = flatPos.magnitude < controller.avoidCenterTurnDist
            ? new Vector3(flatPos.x, 0, flatPos.y).normalized
            : Vector3.zero;

        Vector3 force =
            separationForce * controller.separationWeight +
            cohesionForce * controller.cohesionWeight +
            alignmentForce * controller.alignmentWeight +
            avoidWallsForce * controller.avoidWallsWeight +
            avoidCenterForce * controller.avoidCenterWeight;

        Vector3 velocity = transform.forward * controller.boidSpeed;
        velocity += force * Time.deltaTime;
        velocity = velocity.normalized * controller.boidSpeed;

        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }
}
