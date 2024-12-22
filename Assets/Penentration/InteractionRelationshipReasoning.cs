using UnityEngine;

public class InteractionRelationshipReasoning : MonoBehaviour
{
    [Header("Physics Parameters")]
    [SerializeField] private float objectMass = 1f; // 이 오브젝트의 질량
    [SerializeField] private bool useGravity = true; // 중력 사용 여부

    [Header("Frictions")]
    [SerializeField] private float muS = 0.7f;  // 정적 마찰 계수
    [SerializeField] private float muD = 0.5f;  // 동적 마찰 계수
    [SerializeField] private float frictionScale = 50f; // 마찰력 크기 배율

    [Header("Impulse Parameters")]
    [SerializeField] private float restitutionCoefficient = 0.5f; // 복원 계수 (e)

    private Rigidbody rb;

    void Awake()
    {
        // 이 스크립트가 붙은 오브젝트(손 혹은 물체)에 할당된 Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Rigidbody의 질량과 중력 사용 여부를 인스펙터 값으로 설정
        if (rb != null)
        {
            rb.mass = objectMass;
            rb.useGravity = useGravity;
        }
    }

    // 충돌이 지속되는 동안 매 물리 프레임마다 호출
    void OnCollisionStay(Collision collision)
    {
        // 충돌 중인 모든 접촉점(ContactPoint)에 대해 검사
        foreach (ContactPoint contact in collision.contacts)
        {
            // 상대 오브젝트의 Rigidbody 가져오기
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb == null) continue; // Rigidbody가 없는 경우 마찰 계산 불가

            // 접촉 법선 벡터 계산 (접촉점에서 충돌 방향을 나타냄)
            Vector3 normal = contact.normal.normalized;

            // (1) 충돌 임펄스 계산
            // ---------------------------------------------------------------------------------------
            // 두 오브젝트 간 상대 속도 계산
            Vector3 relativeVelocity = rb.velocity - otherRb.velocity;
            
            // 상대 속도의 법선 방향 성분 계산
            float normalVelocity = Vector3.Dot(relativeVelocity, normal);

            // 충돌 발생 시(법선 속도가 음수일 때만)
            if (normalVelocity < 0)
            {
                // 충돌 임펄스 크기 계산
                float impulseMagnitude = -(1 + restitutionCoefficient) * normalVelocity /
                                          (1 / objectMass + 1 / otherRb.mass);

                // 임펄스 벡터 계산 (법선 방향으로 작용)
                Vector3 impulse = impulseMagnitude * normal;

                // 충돌 상대 오브젝트에 임펄스 적용
                if (!otherRb.isKinematic)
                {
                    otherRb.AddForceAtPosition(impulse, contact.point, ForceMode.Impulse);
                }
                // 현재 오브젝트에 반작용 임펄스 적용
                if (!rb.isKinematic)
                {
                    rb.AddForceAtPosition(-impulse, contact.point, ForceMode.Impulse);
                }
            }

            // (2) 접촉력과 마찰력 계산
            // ---------------------------------------------------------------------------------------
            // 법선 방향의 추정 접촉력 계산 (오브젝트의 중력 기반)
            float approxNormalForce = objectMass * Physics.gravity.magnitude * Mathf.Abs(Vector3.Dot(Vector3.up, normal));

            // 접선 방향 속도 계산 (법선 성분을 제거한 나머지)
            Vector3 tangentVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, normal) * normal;
            float tangentSpeed = tangentVelocity.magnitude;

            if (tangentSpeed < 1e-4f)
            {
                // --- (A) 정적 마찰 처리 ---
                // 속도가 거의 없는 경우 정적 마찰로 간주하며,
                // 정적 마찰력은 외부 힘을 상쇄하는 방향으로 작용
                // 현재는 별도 구현 없이 기본적으로 움직임이 없다고 가정
            }
            else
            {
                // --- (B) 동적 마찰 처리 ---
                // 동적 마찰력 크기 계산 (마찰 계수 * 법선 힘)
                float frictionMag = muD * approxNormalForce;

                // 방어적 처리: 계산 결과가 음수로 나오는 경우 0으로 설정
                if (frictionMag < 0f) frictionMag = 0f;

                // 마찰력 방향: 접선 속도에 반대 방향
                Vector3 frictionDir = -tangentVelocity.normalized;
                Vector3 frictionForce = frictionDir * frictionMag * frictionScale;

                // 상대 오브젝트에 마찰력 적용
                if (!otherRb.isKinematic)
                {
                    otherRb.AddForceAtPosition(frictionForce, contact.point);
                }
                // 현재 오브젝트에 반작용 마찰력 적용
                if (!rb.isKinematic)
                {
                    rb.AddForceAtPosition(-frictionForce, contact.point);
                }
            }
        }
    }

    // 충돌이 시작될 때 호출
    void OnCollisionEnter(Collision collision)
    {
        // 접촉점마다 충돌 임펄스 계산 및 적용
        foreach (ContactPoint contact in collision.contacts)
        {
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb == null) continue; // Rigidbody가 없는 경우 처리 불가

            Vector3 normal = contact.normal.normalized;

            // 두 오브젝트 간 상대 속도 계산
            Vector3 relativeVelocity = rb.velocity - otherRb.velocity;
            float normalVelocity = Vector3.Dot(relativeVelocity, normal);

            // 충돌 발생 시(법선 속도가 음수일 때만)
            if (normalVelocity < 0)
            {
                // 충돌 임펄스 크기 계산
                float impulseMagnitude = -(1 + restitutionCoefficient) * normalVelocity /
                                          (1 / objectMass + 1 / otherRb.mass);

                // 임펄스 벡터 계산 (법선 방향으로 작용)
                Vector3 impulse = impulseMagnitude * normal;

                // 상대 오브젝트에 임펄스 적용
                if (!otherRb.isKinematic)
                {
                    otherRb.AddForceAtPosition(impulse, contact.point, ForceMode.Impulse);
                }
                // 현재 오브젝트에 반작용 임펄스 적용
                if (!rb.isKinematic)
                {
                    rb.AddForceAtPosition(-impulse, contact.point, ForceMode.Impulse);
                }
            }
        }
    }
}

