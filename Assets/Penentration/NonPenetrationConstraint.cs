using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NonPenetrationConstraint : MonoBehaviour
{
    [Header("Constraint Settings")]
    [Tooltip("Stiffness coefficient for non-penetration constraint.")]
    public float stiffnessCoefficient = 10f;

    [Tooltip("Damping coefficient to stabilize the response.")]
    public float dampingCoefficient = 1f;

    private Rigidbody objectRigidbody;

    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        if (objectRigidbody == null)
        {
            Debug.LogError("The object requires a Rigidbody component for physics interaction.");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point; // 위치
            Vector3 normal = contact.normal;     // 접촉 표면의 법선 벡터

            // 침투 깊이 계산
            float penetrationDepth = Vector3.Dot(contact.separation * normal, normal);

            if (penetrationDepth < 0) // 침투가 발생한 경우
            {
                // 비침투 에너지 항 계산
                float penetrationEnergy = -stiffnessCoefficient * penetrationDepth;

                // 복원력 계산 (에너지 그래디언트)
                Vector3 restorationForce = penetrationEnergy * normal;

                // 감쇠력 추가
                Vector3 relativeVelocity = collision.relativeVelocity;
                Vector3 dampingForce = -dampingCoefficient * Vector3.Dot(relativeVelocity, normal) * normal;

                // 최종 힘 적용
                Vector3 totalForce = restorationForce + dampingForce;
                objectRigidbody.AddForceAtPosition(totalForce, contactPoint);
            }
        }
    }
}
