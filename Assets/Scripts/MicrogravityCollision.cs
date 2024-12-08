using UnityEngine;

public class MicrogravityCollision : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            Vector3 relativeVelocity = collision.relativeVelocity;
            Vector3 collisionNormal = collision.contacts[0].normal;
            float restitutionCoefficient = 0.95f; 

            Vector3 bounceForce = -relativeVelocity * restitutionCoefficient;
            rb.AddForce(bounceForce, ForceMode.Impulse);

            Debug.Log($"Collision detected with {collision.gameObject.name}. Bounce applied: {bounceForce}");
        }
    }
}
