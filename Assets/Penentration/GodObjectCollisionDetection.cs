using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GodObjectCollisionDetection : MonoBehaviour
{
    // 비침투 강성 계수: 값이 클수록 겹침 발생 시 물체를 강하게 밀어냄
    public float kN = 500f;  
    private Rigidbody handRigidbody;

    void Awake()
    {
        handRigidbody = GetComponent<Rigidbody>();
    }

    // OnCollisionStay: 매 물리 프레임마다 충돌 중인 접점에 대해 콜백 호출
    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal;

            Collider thisCol = contact.thisCollider;
            Collider otherCol = contact.otherCollider;

            Vector3 direction;
            float distance;

            // Physics.ComputePenetration: 두 콜라이더가 얼마나 겹쳤는지 계산
            bool overlapped = Physics.ComputePenetration(
                thisCol, thisCol.transform.position, thisCol.transform.rotation,
                otherCol, otherCol.transform.position, otherCol.transform.rotation,
                out direction, out distance
            );

            if (overlapped && distance > 0f)
            {
                // 복원력 계산 (F_n = kN * penetration depth * direction)
                Vector3 restoreForce = kN * distance * direction;
                Rigidbody otherRb = collision.rigidbody;

                if (otherRb != null && !otherRb.isKinematic)
                {
                    // 충돌 속도 계산 (v = n \cdot (\dot{c1} - \dot{c2))
                    Vector3 relativeVelocity = handRigidbody != null ? handRigidbody.linearVelocity - otherRb.linearVelocity : -otherRb.linearVelocity;
                    float normalVelocity = Vector3.Dot(normal, relativeVelocity);

                    if (normalVelocity < 0)
                    {
                        // 물리적 충돌 반응 계산
                        otherRb.AddForceAtPosition(restoreForce, contactPoint);
                    }
                }

                if (handRigidbody != null && !handRigidbody.isKinematic)
                {
                    // 반작용력 적용
                    handRigidbody.AddForceAtPosition(-restoreForce, contactPoint);
                }
            }
        }
    }
    // 충돌 감지 상태 업데이트 (God-Object 접근 반영)
    private void UpdateContactPoints(Collider otherCol, Vector3 contactPoint, Vector3 normal)
    {
        Vector3 closestPoint = otherCol.ClosestPoint(contactPoint);
        float penetrationDepth = Vector3.Distance(contactPoint, closestPoint);

        if (penetrationDepth > 0f)
        {
            // 새로운 접촉점으로 업데이트
            Vector3 direction = (contactPoint - closestPoint).normalized;
            Vector3 restoreForce = kN * penetrationDepth * direction;

            if (handRigidbody != null && !handRigidbody.isKinematic)
            {
                handRigidbody.AddForce(restoreForce);
            }
        }
    }
}
