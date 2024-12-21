using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomCollisionSolver : MonoBehaviour
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
        // 충돌 중인 모든 접촉점(ContactPoint)에 대해 반복
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 contactPoint = contact.point;
            //normal은 "이 접촉점에서 충돌 대상 오브젝트가 튕겨나가야 할 방향(반대 오브젝트 기준)"을 반영.
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
                // 복원력 (restoreForce) 계산 : F_n = kN * (침투 깊이) * (침투 벗어나는 방향)
                // direction은 침투를 벗어나기 위한 단위 벡터이며, distance는 penetration depth를 의미.
                Vector3 restoreForce = kN * distance * direction;
                Rigidbody otherRb = collision.rigidbody;
                if (otherRb != null && otherRb.isKinematic == false)
                {
                    // AddForceAtPosition: 특정 지점(contactPoint)에 힘을 가할 수 있어 회전 모멘트까지 반영 가능.
                    otherRb.AddForceAtPosition(restoreForce, contactPoint);
                }
                
                // Kinematic으로 놓는 경우 손이 움직이지 않음. 만약 손을 Dynamic으로 설정했다면, 이 힘으로 인해 손도 충돌에 반응하게 할 수 있음.
                if (handRigidbody != null && handRigidbody.isKinematic == false)
                {
                    // 반작용력 = -restoreForce
                    handRigidbody.AddForceAtPosition(-restoreForce, contactPoint);
                }
            }
        }
    }
}
