using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomCollisionSolver : MonoBehaviour
{
 // 비침투 강성 계수: 값이 클수록 겹침 발생 시 물체를 강하게 밀어냄
    public float kN = 500f;  
    private Rigidbody handRigidbody;

    void Awake()
    {
        // 이 스크립트가 붙은 오브젝트(손)에서 Rigidbody 참조
        handRigidbody = GetComponent<Rigidbody>();
    }

    // OnCollisionStay: 매 물리 프레임마다 충돌 중인 접점에 대해 콜백 호출
    // 이 함수에서 접촉점을 검사하고, 필요한 경우 복원력 적용
    void OnCollisionStay(Collision collision)
    {
        // 충돌 중인 모든 접촉점(ContactPoint)에 대해 반복
        foreach (ContactPoint contact in collision.contacts)
        {
            // contact.point: 접촉지점 월드 좌표
            Vector3 contactPoint = contact.point;

            // contact.normal: 접촉면의 법선 벡터
            // Unity에서 normal은 "이 접촉점에서 충돌 대상 오브젝트가
            // 튕겨나가야 할 방향(반대 오브젝트 기준)"을 반영.
            Vector3 normal = contact.normal; 

            // 충돌한 두 Collider를 가져옴
            Collider thisCol = contact.thisCollider;
            Collider otherCol = contact.otherCollider;

            // direction, distance: 침투 해결을 위해 오브젝트를
            // 어느 방향(direction)으로 distance만큼 이동하면 겹침 해소 가능한지를 알려줌
            Vector3 direction;
            float distance;

            // Physics.ComputePenetration: 두 콜라이더가 얼마나 겹쳤는지 계산
            bool overlapped = Physics.ComputePenetration(
                thisCol, thisCol.transform.position, thisCol.transform.rotation,
                otherCol, otherCol.transform.position, otherCol.transform.rotation,
                out direction, out distance
            );

            // overlapped=true면 콜라이더들이 겹쳐 있는 상태,
            // distance>0이면 실제 침투 깊이가 존재한다는 뜻
            if (overlapped && distance > 0f)
            {
                // 복원력 (restoreForce) 계산
                // F_n = kN * (침투 깊이) * (침투 벗어나는 방향)
                // direction은 침투를 벗어나기 위한 단위 벡터이며,
                // distance는 penetration depth를 의미.
                Vector3 restoreForce = kN * distance * direction;

                // 충돌한 상대 오브젝트의 Rigidbody 참조
                Rigidbody otherRb = collision.rigidbody;
                if (otherRb != null && otherRb.isKinematic == false)
                {
                    // 상대 오브젝트가 Dynamic이라면, 이 힘을 적용해 오브젝트를 밀어냄
                    // AddForceAtPosition: 특정 지점(contactPoint)에 힘을 가할 수 있어
                    // 회전 모멘트까지 반영 가능.
                    otherRb.AddForceAtPosition(restoreForce, contactPoint);
                }

                // 손에도 반작용(force)을 줄 수 있음.
                // 하지만 일반적으로 손을 Kinematic으로 놓는 경우 손이 움직이지 않음.
                // 만약 손을 Dynamic으로 설정했다면, 이 힘으로 인해 손도 충돌에 반응하게 할 수 있음.
                if (handRigidbody != null && handRigidbody.isKinematic == false)
                {
                    // 반작용력 = -restoreForce
                    handRigidbody.AddForceAtPosition(-restoreForce, contactPoint);
                }
            }
        }
    }
}
