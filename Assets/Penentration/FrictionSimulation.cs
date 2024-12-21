using UnityEngine;

public class FrictionSimulation : MonoBehaviour
{ 
    public float staticFrictionCoefficient = 0.5f;  // 정적 마찰 계수
    public float dynamicFrictionCoefficient = 0.3f; // 동적 마찰 계수
    public float normalForce = 10f; // 법선 방향 힘 (예: 중력의 크기)

    private Rigidbody rb;
    private Vector3 appliedForce; // 외부에서 가해진 힘
    private Vector3 velocity;     // 현재 속도

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // 외부에서 가해진 힘 계산 (예: 사용자 입력)
        appliedForce = GetInputForce();

        // 마찰 계산
        Vector3 frictionForce = CalculateFriction();

        // Rigidbody에 힘 적용
        rb.AddForce(appliedForce + frictionForce);
    }

    // 사용자 입력으로 외부 힘 계산 (예: 키보드 입력)
    private Vector3 GetInputForce()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        return new Vector3(inputX, 0, inputZ) * 10f; // 예: 10N 크기의 힘
    }

    // 마찰력 계산
    private Vector3 CalculateFriction()
    {
        // 현재 속도 가져오기
        velocity = rb.linearVelocity;

        // 상대 속도가 0에 가까우면 정적 마찰
        if (velocity.magnitude < 0.01f) // 정적 마찰 기준
        {
            // 정적 마찰력이 appliedForce를 상쇄할 수 있으면 적용
            float maxStaticFriction = staticFrictionCoefficient * normalForce;
            if (appliedForce.magnitude <= maxStaticFriction)
            {
                return -appliedForce; // 정적 마찰력: appliedForce 상쇄
            }
            else
            {
                // 정적 마찰력을 초과하면 동적 마찰로 전환
                return -appliedForce.normalized * maxStaticFriction;
            }
        }
        else
        {
            // 상대 움직임이 발생한 경우 동적 마찰
            float dynamicFriction = dynamicFrictionCoefficient * normalForce;
            return -velocity.normalized * dynamicFriction; // 동적 마찰력: 움직임 반대 방향
        }
    }

    // 시각적 디버깅을 위한 Gizmos (접촉 지점 표시)
    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + velocity * 0.5f); // 속도 벡터 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + appliedForce * 0.1f); // 힘 벡터 표시
        }
    }
}
