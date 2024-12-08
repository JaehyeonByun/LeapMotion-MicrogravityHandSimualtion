using UnityEngine;

public class MicrogravityInteraction : MonoBehaviour
{
    public Transform[] trackedJoints; // Tracked Hand의 각 관절
    public Transform[] renderedJoints; // Rendered Hand의 각 관절
    public LayerMask interactionLayer; // 상호작용 레이어
    public float restitutionCoefficient = 0.8f; // 탄성 계수
    public float springConstant = 100.0f; // 접촉 스프링 상수
    public float staticFriction = 0.6f; // 정지 마찰 계수
    public float dynamicFriction = 0.4f; // 동적 마찰 계수

    void Update()
    {
        for (int i = 0; i < trackedJoints.Length; i++)
        {
            HandleInteraction(trackedJoints[i], renderedJoints[i]);
        }
    }

    void HandleInteraction(Transform trackedJoint, Transform renderedJoint)
    {
        // 충돌 감지
        Collider[] hitColliders = Physics.OverlapSphere(trackedJoint.position, 0.02f, interactionLayer);
        if (hitColliders.Length > 0)
        {
            Collider collidingObject = hitColliders[0];
            Vector3 contactPoint = collidingObject.ClosestPoint(trackedJoint.position);
            Vector3 contactNormal = (trackedJoint.position - contactPoint).normalized;

            // Impulse Phase
            float relativeVelocity = Vector3.Dot(trackedJoint.GetComponent<Rigidbody>().linearVelocity, contactNormal);
            float impulseMagnitude = -(1 + restitutionCoefficient) * relativeVelocity;
            impulseMagnitude /= Vector3.Dot(contactNormal, contactNormal / collidingObject.GetComponent<Rigidbody>().mass);
            Vector3 impulse = impulseMagnitude * contactNormal;
            collidingObject.GetComponent<Rigidbody>().linearVelocity += impulse / collidingObject.GetComponent<Rigidbody>().mass;

            // Penetration Phase
            Vector3 contactForce = springConstant * (contactPoint - trackedJoint.position);
            Vector3 normalForce = Vector3.Dot(contactForce, contactNormal) * contactNormal;
            Vector3 tangentForce = contactForce - normalForce;

            // 마찰 모델
            Vector3 frictionForce;
            if (tangentForce.magnitude <= staticFriction * normalForce.magnitude)
            {
                frictionForce = tangentForce; // 정지 마찰
            }
            else
            {
                frictionForce = dynamicFriction * tangentForce.normalized * normalForce.magnitude; // 동적 마찰
            }

            // Rendered Joint 업데이트
            renderedJoint.position = contactPoint + contactNormal * 0.01f;
            renderedJoint.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            // 충돌이 없을 경우 Rendered Joint 복원
            renderedJoint.position = trackedJoint.position;
            renderedJoint.GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
