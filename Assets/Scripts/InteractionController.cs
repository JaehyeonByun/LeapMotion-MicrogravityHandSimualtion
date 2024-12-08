using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private PhysicsSimulation physicsSimulation;

    void Start()
    {
        physicsSimulation = GetComponent<PhysicsSimulation>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 contactNormal = collision.contacts[0].normal;
        Vector3 relativeVelocity = collision.relativeVelocity;

        physicsSimulation.SimulateCollision(relativeVelocity, contactNormal);
    }
}
