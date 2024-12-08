using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public Transform trackedHand;
    public LayerMask collisionLayer;

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(trackedHand.position, 0.05f, collisionLayer);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 contactPoint = hitCollider.ClosestPoint(trackedHand.position);
            Debug.Log("Collision detected at: " + contactPoint);
        }
    }
}
