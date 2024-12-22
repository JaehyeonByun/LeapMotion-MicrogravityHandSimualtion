using UnityEngine;

public class GodObjectCollisionDetection : MonoBehaviour
{
  public Transform handTracking;  // 트래킹된 손의 Transform
    public Transform handRenderer; // 렌더링된 손의 Transform

    public float penetrationThreshold = 0.01f; // 관통 허용 임계값
    public LayerMask collisionLayer; // 충돌 감지 레이어

    private Vector3 contactPoint;
    private Vector3 contactNormal;
    private bool isColliding = false;

    void Update()
    {
        // Step 1: 충돌 감지
        DetectCollision();

        // Step 2: God-Object 업데이트
        if (isColliding)
        {
            UpdateRendererPosition();
        }
        else
        {
            AlignRendererWithTracking();
        }
    }

    private void DetectCollision()
    {
        // Raycast를 사용하여 handTracking 오브젝트와의 충돌을 감지
        Ray ray = new Ray(handTracking.position, handTracking.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, penetrationThreshold, collisionLayer))
        {
            isColliding = true;
            contactPoint = hit.point;
            contactNormal = hit.normal;
        }
        else
        {
            isColliding = false;
        }
    }

    private void UpdateRendererPosition()
    {
        // 렌더링된 손의 위치를 업데이트하여 관통을 방지
        Vector3 penetrationDepth = contactPoint - handTracking.position;
        Vector3 correctedPosition = handTracking.position + penetrationDepth.normalized * penetrationThreshold;

        handRenderer.position = correctedPosition;

        // 렌더링된 손의 방향을 표면의 법선 벡터와 정렬
        handRenderer.rotation = Quaternion.LookRotation(-contactNormal, Vector3.up);
    }

    private void AlignRendererWithTracking()
    {
        // 충돌이 없을 때 렌더링된 손을 트래킹된 손과 정렬
        handRenderer.position = handTracking.position;
        handRenderer.rotation = handTracking.rotation;
    }

    void OnDrawGizmos()
    {
        if (isColliding)
        {
            // 충돌 지점과 표면 법선 시각화
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(contactPoint, 0.01f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(contactPoint, contactPoint + contactNormal * 0.1f);
        }
    }
}
