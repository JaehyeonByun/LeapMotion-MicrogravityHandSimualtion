using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NonPenetrationConstraint : MonoBehaviour
{
    // 강성 계수와 마찰 계수
    public float stiffness = 1000f; // k_n 값
    public float penetrationThreshold = 0.01f; // 허용 침투 거리

    // 접촉 표면의 법선 벡터 계산
    private Vector3 CalculateNormal(Vector3 contactPoint, Collider other)
    {
        Vector3 normal = (contactPoint - other.ClosestPoint(contactPoint)).normalized;
        return normal;
    }

    // 복원력 계산
    private Vector3 CalculateRestorationForce(Vector3 contactPoint, Vector3 objectAnchor, Vector3 normal)
    {
        float penetrationDepth = Vector3.Dot(objectAnchor - contactPoint, normal);
        if (penetrationDepth > 0) return Vector3.zero; // 침투가 없으면 힘 없음
        return -stiffness * penetrationDepth * normal; // 복원력 계산
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 접촉 지점 정보
            Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal;

            // 객체의 표면 앵커 계산
            Vector3 objectAnchor = collision.collider.ClosestPoint(contactPoint);

            // 복원력 계산
            Vector3 restorationForce = CalculateRestorationForce(contactPoint, objectAnchor, normal);

            // Rigidbody에 복원력 적용
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(restorationForce, ForceMode.Force);
            }
        }
    }
}
