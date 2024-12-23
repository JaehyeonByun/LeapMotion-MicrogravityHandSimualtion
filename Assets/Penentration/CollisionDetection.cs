using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public GameObject hand;  
    public GameObject obj;   

    [Header("Physics Parameters")]
    public float restitution = 0.5f; 

    private Rigidbody handRb;
    private Rigidbody objRb;

    private Vector3 contactPoint;
    private Vector3 normalVector;  

    void Start()
    {
        handRb = hand.GetComponent<Rigidbody>();
        objRb = obj.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == obj)
        {
            ContactPoint contact = collision.GetContact(0);
            contactPoint = contact.point;  
            normalVector = contact.normal; 

            CalculateContactVelocity();
        }
    }

    void CalculateContactVelocity()
    {
        
        Vector3 handCenter = handRb.worldCenterOfMass;  
        Vector3 objCenter = objRb.worldCenterOfMass;   

       
        Vector3 r1 = contactPoint - handCenter;  
        Vector3 r2 = contactPoint - objCenter;   

       
        Vector3 handVelocity = handRb.linearVelocity + Vector3.Cross(handRb.angularVelocity, r1);
        Vector3 objVelocity = objRb.linearVelocity + Vector3.Cross(objRb.angularVelocity, r2);

       
        Vector3 relativeVelocity = handVelocity - objVelocity;
        float normalSpeed = Vector3.Dot(normalVector, relativeVelocity);

    }
}
