using UnityEngine;
using System.Collections.Generic;
public class HandGodObject : MonoBehaviour
{
    [Header("Tracked Joints")] // 트래킹된 손 관절을 직접 할당
    public List<Transform> trackedJoints;

    [Header("Rendered Joints")] // 렌더링된 손 관절을 직접 할당
    public List<Transform> renderedJoints;

    public float penetrationDepthThreshold = 0.01f; // 관통 깊이 임계값
    public float smoothingFactor = 0.1f; // 렌더링 손 움직임 부드럽게 처리
    public LayerMask collisionLayer; // 충돌 감지 레이어

    private void Update()
    {
        for (int i = 0; i < trackedJoints.Count; i++)
        {
            Transform trackedJoint = trackedJoints[i];
            Transform renderedJoint = renderedJoints[i];

            Vector3 closestPoint;
            bool isColliding = CheckCollision(trackedJoint, out closestPoint);

            if (isColliding)
            {
                Debug.Log("UpdateRenderedJoint");
                UpdateRenderedJoint(renderedJoint, closestPoint, trackedJoint);
            }
            else
            {
                AlignRenderedJoint(renderedJoint, trackedJoint);
            }
        }
    }

    private bool CheckCollision(Transform joint, out Vector3 closestPoint)
    {
        closestPoint = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(joint.position, 0.01f, collisionLayer);

        foreach (var collider in colliders)
        {
            closestPoint = collider.ClosestPoint(joint.position);
            float penetrationDepth = Vector3.Distance(joint.position, closestPoint);

            if (penetrationDepth <= penetrationDepthThreshold)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateRenderedJoint(Transform renderedJoint, Vector3 closestPoint, Transform trackedJoint)
    {
        renderedJoint.position = Vector3.Lerp(renderedJoint.position, closestPoint, smoothingFactor);

        Vector3 normalDirection = (renderedJoint.position - trackedJoint.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(normalDirection, Vector3.up);
        renderedJoint.rotation = Quaternion.Lerp(renderedJoint.rotation, targetRotation, smoothingFactor);
    }

    private void AlignRenderedJoint(Transform renderedJoint, Transform trackedJoint)
    {
        renderedJoint.position = Vector3.Lerp(renderedJoint.position, trackedJoint.position, smoothingFactor);
        renderedJoint.rotation = Quaternion.Lerp(renderedJoint.rotation, trackedJoint.rotation, smoothingFactor);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var joint in trackedJoints)
        {
            Gizmos.DrawWireSphere(joint.position, 0.01f);
        }
    }
}
