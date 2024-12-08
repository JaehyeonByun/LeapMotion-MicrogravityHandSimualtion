using UnityEngine;

public class PhysicsSimulation : MonoBehaviour
{
    [Header("Physics Coefficients")]
    [SerializeField] private float mass = 1.0f;                  // 물체 질량
    [SerializeField] private float frictionCoefficient = 0.5f;  // 마찰 계수
    [SerializeField] private float restitutionCoefficient = 0.8f; // 반발 계수

    private Rigidbody objectRb;

    void Start()
    {
        objectRb = GetComponent<Rigidbody>();
        objectRb.mass = mass;
    }

    public void SimulateCollision(Vector3 relativeVelocity, Vector3 contactNormal)
    {
        Vector3 impulse = CalculateImpulse(relativeVelocity, contactNormal);
        objectRb.AddForce(impulse, ForceMode.Impulse);
    }

    private Vector3 CalculateImpulse(Vector3 velocity, Vector3 normal)
    {
        float relativeVelocity = Vector3.Dot(velocity, normal);
        return -normal * (1 + restitutionCoefficient) * relativeVelocity / mass;
    }

    public Vector3 CalculateFriction(Vector3 normalForce, Vector3 tangentialVelocity)
    {
        float maxFriction = normalForce.magnitude * frictionCoefficient;

        if (tangentialVelocity.magnitude > maxFriction)
        {
            return tangentialVelocity.normalized * maxFriction; // 동적 마찰
        }
        return tangentialVelocity; 
    }
}
