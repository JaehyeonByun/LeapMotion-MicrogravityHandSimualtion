using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// ����� �ڵ� ���� ������ �� �ΰ� �� ����
public class AddPhysicsToHand : MonoBehaviour
{
    void Start()
    {
        // ���� ��� Transform �ڽĵ鿡 ���� ó��
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            // Rigidbody �߰� (�̹� ������ �ǳʶ�)
            if (child.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            // Collider �߰� (�̹� ������ �ǳʶ�)
            if (child.GetComponent<Collider>() == null)
            {
                // �չٴڰ� �հ����� ������ Collider ����
                if (child.name.Contains("palm"))
                {
                    BoxCollider box = child.gameObject.AddComponent<BoxCollider>();
                    box.size = new Vector3(0.1f, 0.02f, 0.1f);
                }
                else if (child.name.Contains("finger") || child.name.Contains("thumb"))
                {
                    CapsuleCollider capsule = child.gameObject.AddComponent<CapsuleCollider>();
                    capsule.radius = 0.005f;
                    capsule.height = 0.04f;
                    capsule.direction = 2; // Z-Axis
                }
            }
        }
    }
}
