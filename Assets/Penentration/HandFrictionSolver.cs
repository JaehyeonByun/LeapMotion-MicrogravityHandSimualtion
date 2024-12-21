using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HandFrictionSolver : MonoBehaviour
{
    [Header("마찰 계수들")]
    public float muS = 0.7f;   
    public float muD = 0.5f;  
    public float kFriction = 50f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal.normalized;
            Rigidbody otherRb = collision.rigidbody;
            if (otherRb == null) continue;

            Vector3 relativeVel = rb.velocity - otherRb.velocity;
            
            // 1) 정상력 (법선 방향 속도/힘)
            //    여기서는 간단히 '상대 속도의 법선 방향 성분'이나
            //    이미 충돌 처리에서 구해둔 법선 힘(restoreForce) 등을 참고할 수 있음.
            //    혹은 rigidbody.mass, gravity 등을 고려해 실제 접촉 법선 힘을 추정해야 함.
            float normalForce = Vector3.Dot(Vector3.down * rb.mass * Physics.gravity.magnitude, normal);
            // 실제 상황에서는 물체의 무게, 손가락이 누르는 힘 등 더 정확한 계산이 필요

            // 2) 접선 방향 속도 (마찰이 발생할 축)
            Vector3 tangentVel = relativeVel - Vector3.Dot(relativeVel, normal) * normal;
            float tangentSpeed = tangentVel.magnitude;
            if (tangentSpeed < 1e-4) {
                // 거의 미끄러지지 않음(정적 마찰 구간인지 체크)
                // 정적 마찰 원뿔 조건: |Ft| <= mu_s * |Fn|
                // 여기서는 '외부에서 걸리는 접선방향 힘'을 추정해야 하지만
                // 간단히 속도가 거의 0이면 마찰력이 모든 외부 접선힘을 상쇄한다고 가정
                // 최대 정적 마찰력을 초과하는 힘이 걸리면 -> 동적 마찰 전환
            }
            else
            {
                // 동적 마찰력 계산 (Coulomb)
                // F_dynamic = mu_d * F_n (접선 방향) 
                // 실제 구현에서는 '상대 속도 방향 반대'로 힘을 적용
                Vector3 frictionDir = -tangentVel.normalized;
                float frictionMag = muD * normalForce;

                // 실제론 normalForce가 음수가 되지 않게 max(0, ~) 등 보정 필요
                if (frictionMag < 0f) frictionMag = 0f;

                // 최종 마찰력
                Vector3 frictionForce = frictionDir * frictionMag * kFriction;

                // 물체와 손에 반대 방향으로 적용
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
