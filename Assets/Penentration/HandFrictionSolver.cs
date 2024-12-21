using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HandFrictionSolver : MonoBehaviour
{
    [Header("Physics Parameters")]
    [SerializeField] private float objectMass = 1f; // 이 오브젝트의 질량
    [SerializeField] private bool useGravity = true; // 중력 사용 여부

    [Header("Frictions")]
    [SerializeField] private float muS = 0.7f;  // 정적 마찰 계수
    [SerializeField] private float muD = 0.5f;  // 동적 마찰 계수
    [SerializeField] private float frictionScale = 50f; // 마찰력 크기 배율

    private Rigidbody rb;

    void Awake()
    {
        // 이 스크립트가 붙은 오브젝트(손 혹은 물체)에 할당된 Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // 인스펙터에서 설정한 값들을 Rigidbody에 반영
        if (rb != null)
        {
            rb.mass = objectMass;
            rb.useGravity = useGravity;
        }
    }

    // 충돌이 계속되는 동안 매 물리 프레임마다 호출
    void OnCollisionStay(Collision collision)
    {
        // 충돌 중인 모든 접촉점(ContactPoint)에 대해 검사
        foreach (ContactPoint contact in collision.contacts)
        {
            // 상대방 Rigidbody가 있는지 확인 (없으면 마찰 계산 불가)
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb == null) continue;

            // 접촉 법선 벡터(보통 otherRb -> this 쪽 방향)
            Vector3 normal = contact.normal.normalized;

            // (1) 정상력(Normal Force) 추정
            // ---------------------------------------------------------------------------------------
            // 실제론 충돌 임펄스나 표면 압력을 통해 정확히 구해야 하나,
            // 여기서는 간단히 "오브젝트 질량 × 중력"이 법선 방향으로 작용한다고 가정.
            // 또한 Vector3.Dot()를 사용해 법선에 수직 방향으로만 계산되도록 함.
            // ※ 프로젝트에 따라 손가락이 누르는 힘, 접촉 각도 등을 종합해야 함.
            float approxNormalForce = objectMass * Physics.gravity.magnitude * Mathf.Abs(Vector3.Dot(Vector3.up, normal));

            // (2) 상대 속도(접선 성분) 계산 -> 마찰 발생 축
            // ---------------------------------------------------------------------------------------
            // 두 Rigidbody 간 속도 차
            Vector3 relativeVelocity = rb.linearVelocity - otherRb.linearVelocity;

            // normal 방향 성분을 뺀 나머지가 '접선' 방향 속도
            Vector3 tangentVelocity = relativeVelocity - Vector3.Dot(relativeVelocity, normal) * normal;
            float tangentSpeed = tangentVelocity.magnitude;

            // (3) 정적/동적 마찰 판별 및 힘 적용
            // ---------------------------------------------------------------------------------------
            // 간단히 속도가 아주 작으면 '정적 마찰'로 간주, 움직이면 '동적 마찰'로 간주
            if (tangentSpeed < 1e-4f)
            {
                // --- (A) 정적 마찰 영역 ---
                // 실제 구현에서는 정적 마찰력 F_static <= muS * N 이라는 한도 내에서
                // 외부 힘을 상쇄하는 방향으로 동작.
                // 여기서는 "속도가 0이면 이미 움직이지 않고, 마찰이 모든 힘을 상쇄하고 있다"
                // 라고 단순 가정하고 별도 힘을 적용하지 않음(혹은 최소한으로 적용).

                // 필요 시, "외부 힘 추정 -> 정적 마찰 한계 초과 시 동적 전환" 로직을 추가할 수 있음.
            }
            else
            {
                // --- (B) 동적 마찰 영역 ---
                // 동적 마찰력 = muD * N, 방향은 상대속도에 반대
                float frictionMag = muD * approxNormalForce;

                // 혹시 계산 결과가 음수로 가는 상황(중력 방향 반대 등)을 방어적으로 처리
                if (frictionMag < 0f) frictionMag = 0f;

                // 마찰력 방향: 상대속도에 반대 방향
                Vector3 frictionDir = -tangentVelocity.normalized;
                Vector3 frictionForce = frictionDir * frictionMag * frictionScale;

                // (4) 힘 적용
                // ---------------------------------------------------------------------------------------
                // - 상대 오브젝트(otherRb)는 마찰력(frictionForce)를
                // - 현재 오브젝트(rb)는 반작용력(-frictionForce)를 적용
                if (!otherRb.isKinematic)
                {
                    otherRb.AddForceAtPosition(frictionForce, contact.point);
                }
                if (!rb.isKinematic)
                {
                    rb.AddForceAtPosition(-frictionForce, contact.point);
                }
            }
        }
    }
}
