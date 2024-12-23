using UnityEngine;
using System.Collections.Generic;
public class HandGodObject1 : MonoBehaviour
{
    [Header("Tracked Joints")]
    public List<Transform> trackedJoints;

    [Header("Rendered Joints")]
    public List<Transform> renderedJoints;

    [Header("God Object")]
    public List<Transform> godObject; 

    public float penetrationDepthThreshold = 0.01f;
    public LayerMask collisionLayer;

    void Update()
    {
        UpdateGodObjectPosition(); //위치 업데이트

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

    private void UpdateGodObjectPosition() //위치 업데이트
    {
        if (godObject == null || godObject.Count==0) return;

        Vector3 updatedPosition = trackedJoints[0].position; //기준 위치

        foreach (Transform joint in trackedJoints)
        {
            Collider[] colliders = Physics.OverlapSphere(joint.position, 0.01f, collisionLayer);
            if (colliders.Length > 0)
            {
              
                updatedPosition = trackedJoints[0].position;
                break;
            }
        }

        godObject[0].position = updatedPosition;
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
        renderedJoint.position = closestPoint; 
        renderedJoint.rotation = Quaternion.Euler(270, 0, 0); 
    }

    private void AlignRenderedJoint(Transform renderedJoint, Transform trackedJoint)
    {
        renderedJoint.position = trackedJoint.position; 
        renderedJoint.rotation = trackedJoint.rotation * Quaternion.Euler(270, 0, 0);
    }
}
