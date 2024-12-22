using UnityEngine;

// 충격량 벡터 - 빨간색
// 법선 힘 벡터 - 파란색
// 정치마찰 초록색 / 동적 마찰 노란색

[RequireComponent(typeof(Rigidbody))]
public class InteractionRelationshipReasoning : MonoBehaviour
{
    // 복원 계수: (0 = 완전 비탄성, 1 = 완전 탄성)
    public float restitutionCoefficient = 0.1f;
    // 강성 계수: 접촉 강도를 조절, 값이 클수록 강한 접촉력 생성
    public float gamma = 50f;

    public float staticFrictionCoefficient = 0.7f;
    public float dynamicFrictionCoefficient = 0.2f;

    private Rigidbody objectRigidbody;

    void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point; // 접촉 지점 좌표
            Vector3 normal = contact.normal; // 접촉 표면 법선 벡터

            Rigidbody handRigidbody = collision.rigidbody;

            if (handRigidbody != null)
            {
                // 임펄스 기반 상호작용 (Impulse Phase)
                Vector3 relativeVelocity = objectRigidbody.linearVelocity - handRigidbody.linearVelocity; // 상대 속도
                float normalVelocity = Vector3.Dot(relativeVelocity, normal); // 법선 방향 속도

                if (normalVelocity < 0)
                {
                    float impulseMagnitude = -(1 + restitutionCoefficient) * normalVelocity / (Vector3.Dot(normal, normal) * (1 / objectRigidbody.mass + 1 / handRigidbody.mass));

                    // 충격량 벡터(J) 계산
                    Vector3 impulse = impulseMagnitude * normal;

                    // 물체에 충격량 적용
                    objectRigidbody.AddForceAtPosition(impulse, contactPoint, ForceMode.Impulse);

                    // 손 객체에 반작용 힘 적용
                    handRigidbody.AddForceAtPosition(-impulse, contactPoint, ForceMode.Impulse);

                    // 충격량 시각화
                    Debug.Log($"Impulse applied at {contactPoint}: {impulse}");
                }

                // 침투 상호작용 (Penetration Phase)
                Vector3 contactForce = gamma * (contactPoint - transform.position); // 접촉 강성에 따른 힘 계산
                Vector3 normalForce = Vector3.Dot(contactForce, normal) * normal; // 법선 성분 분리
                Vector3 tangentialForce = contactForce - normalForce; // 접선 성분 분리

                // 법선 힘 시각화
                Debug.Log($"Normal force at {contactPoint}: {normalForce}");

                if (tangentialForce.magnitude <= staticFrictionCoefficient * normalForce.magnitude)
                {
                    // 정지 마찰 상태
                    objectRigidbody.AddForceAtPosition(-tangentialForce, contactPoint, ForceMode.Force);

                    // 접선 힘 시각화 (정지 마찰)
                    Debug.DrawLine(contactPoint, contactPoint - tangentialForce, Color.green, 0.5f);
                    Debug.Log($"Static friction applied at {contactPoint}: {-tangentialForce}");
                }
                else
                {
                    // 동적 마찰 상태
                    Vector3 dynamicFrictionForce =
                        tangentialForce.normalized * dynamicFrictionCoefficient * normalForce.magnitude;
                    objectRigidbody.AddForceAtPosition(-dynamicFrictionForce, contactPoint, ForceMode.Force);

                    // 접선 힘 시각화 (동적 마찰)
                    Debug.DrawLine(contactPoint, contactPoint - dynamicFrictionForce, Color.yellow, 0.5f);
                    Debug.Log($"Dynamic friction applied at {contactPoint}: {-dynamicFrictionForce}");
                }
            }
        }
    }
}
