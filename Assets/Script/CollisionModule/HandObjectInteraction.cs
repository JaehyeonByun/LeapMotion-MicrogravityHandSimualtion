using UnityEngine;
using System.Collections.Generic;
public class HandObjectInteraction : MonoBehaviour
{
    [SerializeField] private HandGodObject _handGodObject;

    [Header("Tracked Joints")]
    public List<Transform> trackedJoints;

    [Header("Virtual Object")]
    public Rigidbody virtualObject; 
    public float mass = 0.3f; // 질량
    public float restitutionCoefficient = 0.8f; // 탄성 계수 (e)
    public float frictionCoefficientStatic = 0.1f; // 정지 마찰 계수 (μ)
    public float frictionCoefficientDynamic = 0.05f; // 동적 마찰 계수 (φ)
    public float contactForceMultiplier = 50.0f; // 접촉 힘 계산용 상수

    void Awake()
    {
        if (_handGodObject != null)
        {
            trackedJoints = _handGodObject.GetComponent<HandGodObject>().trackedJoints;
        }
        else
        {
            Debug.LogWarning("_handGodObject가 설정되지 않았습니다.");
        }
    }
    private void FixedUpdate()
    {
        foreach (Transform joint in trackedJoints)
        {
            Vector3 closestPoint;
            if (CheckCollision(joint, out closestPoint))
            {
                Vector3 normal = CalculateNormal(closestPoint, joint.position);
                ApplyImpulsePhase(normal, joint.position, closestPoint);
                ApplyPenetrationPhase(normal, joint.position, closestPoint);
            }
        }
    }

    private bool CheckCollision(Transform joint, out Vector3 closestPoint)
    {
        closestPoint = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(joint.position, 0.01f);

        foreach (Collider collider in colliders)
        {
            if (collider.attachedRigidbody == virtualObject)
            {
                closestPoint = collider.ClosestPoint(joint.position);
                return true;
            }
        }
        return false;
    }

    private Vector3 CalculateNormal(Vector3 contactPoint, Vector3 jointPosition)
    {
        return (contactPoint - jointPosition).normalized;
    }

    private void ApplyImpulsePhase(Vector3 normal, Vector3 jointPosition, Vector3 contactPoint)
    {
        Vector3 relativeVelocity = virtualObject.linearVelocity;
        float normalVelocity = Vector3.Dot(relativeVelocity, normal);

        if (normalVelocity < 0)
        {
            float impulseMagnitude = -(1 + restitutionCoefficient) * normalVelocity / (1 / mass);
            Vector3 impulse = impulseMagnitude * normal;
            virtualObject.AddForce(impulse, ForceMode.Impulse);
        }
    }

    private void ApplyPenetrationPhase(Vector3 normal, Vector3 jointPosition, Vector3 contactPoint)
    {
        Vector3 contactForce = contactForceMultiplier * (contactPoint - jointPosition);
        Vector3 normalForce = Vector3.Dot(contactForce, normal) * normal;
        Vector3 tangentialForce = contactForce - normalForce;

        if (tangentialForce.magnitude <= frictionCoefficientStatic * normalForce.magnitude)
        {
            virtualObject.AddForce(tangentialForce, ForceMode.Force);
        }
        else
        {
            Vector3 slidingForce = frictionCoefficientDynamic * normalForce.magnitude * tangentialForce.normalized;
            virtualObject.AddForce(slidingForce, ForceMode.Force);
        }
    }
}
