using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 립모션 핸드 왼쪽 오른쪽 손 두개 다 적용
public class AddPhysicsToHand : MonoBehaviour
{
    void Start()
    {
        // 손의 모든 Transform 자식들에 대해 처리
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            // Rigidbody 추가 (이미 있으면 건너뜀)
            if (child.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }

            // Collider 추가 (이미 있으면 건너뜀)
            if (child.GetComponent<Collider>() == null)
            {
                // 손바닥과 손가락에 적절한 Collider 선택
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
