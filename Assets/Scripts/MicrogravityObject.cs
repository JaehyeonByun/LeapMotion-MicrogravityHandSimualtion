using UnityEngine;

public class MicrogravityObject : MonoBehaviour
{
    [Header("Physics Coefficients")]
    [SerializeField] private float mass = 1.0f; 
    [SerializeField] private float frictionCoefficient = 0.01f; // 마찰 계수 (낮음)
    [SerializeField] private float restitutionCoefficient = 0.95f; // 반발 계수 (높음)
    [SerializeField] private Vector3 initialImpulse = Vector3.zero; // 초기 속도

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.mass = mass; 
        rb.linearDamping = frictionCoefficient;
        rb.angularDamping = frictionCoefficient; 
        
        if (initialImpulse != Vector3.zero)
        {
            rb.AddForce(initialImpulse, ForceMode.Impulse);
            Debug.Log($"{gameObject.name} initial impulse applied: {initialImpulse}");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;
        Vector3 relativeVelocity = collision.relativeVelocity;
        
        Vector3 bounce = -relativeVelocity * restitutionCoefficient;

        rb.AddForce(bounce, ForceMode.Impulse);
        Debug.Log($"{gameObject.name} bounced with impulse: {bounce}");
    }
}
