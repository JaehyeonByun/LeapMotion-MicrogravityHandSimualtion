using UnityEngine;

public class SetMicrogravity : MonoBehaviour
{
    void Start()
    {
        // 전역 중력을 무중력으로 설정
        Physics.gravity = Vector3.zero;

        Debug.Log("전체 중력이 무중력으로 설정되었습니다.");
    }
}
