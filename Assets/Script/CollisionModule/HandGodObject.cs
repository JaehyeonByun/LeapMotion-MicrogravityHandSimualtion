using UnityEngine;
using System.Collections.Generic;
public class HandGodObject : MonoBehaviour
{
    [Header("Tracked Joints")] // 트래킹된 손 관절을 직접 할당
    public List<Transform> trackedJoints;

    [Header("Rendered Joints")] // 렌더링된 손 관절을 직접 할당
    public List<Transform> renderedJoints;

    public float penetrationDepthThreshold = 0.01f; // 관통 깊이 임계값
    public LayerMask collisionLayer; // 충돌 감지 레이어

    void Update()
    {
        for (int i = 0; i < trackedJoints.Count; i++)
        {
            Transform trackedJoint = trackedJoints[i];
            Transform renderedJoint = renderedJoints[i];

            Vector3 closestPoint;
            bool isColliding = CheckCollision(trackedJoint, out closestPoint);

            if (isColliding)
            {
                UpdateRenderedJoint(renderedJoint, closestPoint);
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

    private void UpdateRenderedJoint(Transform renderedJoint, Vector3 closestPoint)
    {
        // 렌더링 손을 충돌 지점에 위치시킴
        renderedJoint.position = closestPoint;
        renderedJoint.rotation = Quaternion.Euler(270, 0, 0);
    }

    private void AlignRenderedJoint(Transform renderedJoint, Transform trackedJoint)
    {
        // 렌더링 손과 트래킹 손 위치 일치
        renderedJoint.position = trackedJoint.position;
        renderedJoint.rotation = trackedJoint.rotation * Quaternion.Euler(270, 0, 0);
    }
}
