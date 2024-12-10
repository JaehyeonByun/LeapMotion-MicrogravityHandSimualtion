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
        
        UpdateGodObjectPosition();

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

  
    private void UpdateGodObjectPosition()
    {
        if (godObject == null || godObject.Count == 0) return;

        Vector3 godObjectPosition = trackedJoints[0].position;
        godObject[0].position = godObjectPosition;
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
