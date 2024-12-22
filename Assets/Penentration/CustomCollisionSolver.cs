using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Renderer))]
public class CustomCollisionSolver : MonoBehaviour
{
   public float kN = 500f; // 비침투 강성 계수
    public float minPenetration = 0.01f; // 최소 침투 거리
    public float maxForceMagnitude = 1000f; // 최대 복원력 크기
    public float dampingFactor = 0.9f; // 복원력 감쇠 계수

    private Rigidbody handRigidbody;
    private Renderer objectRenderer;
    private Color defaultColor = Color.blue; // 기본 색상 (파란색)
    private Color contactColor = Color.yellow; // 접촉 색상 (노란색)
    private Color penetrationColor = Color.red; // 침투 색상 (빨간색)

    private bool isPenetrating = false;
    private bool isContacting = false;

    void Awake()
    {
        handRigidbody = GetComponent<Rigidbody>();
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material.color = defaultColor; // 초기 색상 설정
    }

    void OnCollisionStay(Collision collision)
    {
        isContacting = false;
        isPenetrating = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point;
            Vector3 normal = contact.normal;

            Collider thisCol = contact.thisCollider;
            Collider otherCol = contact.otherCollider;

            Vector3 direction;
            float distance;

            bool overlapped = Physics.ComputePenetration(
                thisCol, thisCol.transform.position, thisCol.transform.rotation,
                otherCol, otherCol.transform.position, otherCol.transform.rotation,
                out direction, out distance
            );

            if (overlapped)
            {
                isContacting = true;
                if (distance > minPenetration)
                {
                    isPenetrating = true;

                    // 복원력 계산
                    Vector3 restoreForce = kN * distance * direction;

                    // 복원력 감쇠 적용
                    restoreForce *= dampingFactor;

                    // 복원력 크기 제한
                    if (restoreForce.magnitude > maxForceMagnitude)
                    {
                        restoreForce = restoreForce.normalized * maxForceMagnitude;
                    }

                    // 충돌 물체에 복원력 적용
                    Rigidbody otherRb = collision.rigidbody;
                    if (otherRb != null && !otherRb.isKinematic)
                    {
                        otherRb.AddForceAtPosition(restoreForce, contactPoint);
                    }

                    // 현재 물체에도 반작용력 적용
                    if (handRigidbody != null && !handRigidbody.isKinematic)
                    {
                        handRigidbody.AddForceAtPosition(-restoreForce, contactPoint);
                    }

                    // 침투 깊이 시각화
                    Debug.DrawLine(contactPoint, contactPoint + (direction * distance), penetrationColor);

                    // 복원력 시각화
                    Debug.DrawLine(contactPoint, contactPoint + (restoreForce.normalized * 0.1f), Color.blue);
                }
            }
        }

        UpdateColor();
    }

    void OnCollisionExit(Collision collision)
    {
        isContacting = false;
        isPenetrating = false;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (isPenetrating)
        {
            objectRenderer.material.color = penetrationColor; // 침투 중 빨간색
        }
        else if (isContacting)
        {
            objectRenderer.material.color = contactColor; // 접촉 중 노란색
        }
        else
        {
            objectRenderer.material.color = defaultColor; // 충돌 없음 파란색
        }
    }
}
