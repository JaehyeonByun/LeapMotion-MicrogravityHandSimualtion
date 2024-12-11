using System.Collections.Generic;
using UnityEngine;

public class FilteredHandJoints : MonoBehaviour
{
    [Header("Joints to Filter")]
    public List<Transform> jointTransforms; // 손가락 조인트들의 Transform 목록

    [Header("OneEuroFilter Parameters")]
    public float freq = 90.0f;       // 업데이트 주파수 (Leap Motion은 약 90Hz)
    public float minCutoff = 1.0f;   // 최소 컷오프 주파수
    public float beta = 0.007f;      // 속도에 따른 반응도 조정
    public float dCutoff = 1.0f;     // 변화율에 대한 컷오프 주파수

    // 각 조인트에 대응하는 OneEuroFilter 인스턴스
    private Dictionary<Transform, OneEuroFilter<Vector3>> jointFilters;

    void Start()
    {
        // 각 조인트에 대해 OneEuroFilter 초기화
        jointFilters = new Dictionary<Transform, OneEuroFilter<Vector3>>();

        foreach (Transform joint in jointTransforms)
        {
            jointFilters[joint] = new OneEuroFilter<Vector3>(freq, minCutoff, beta, dCutoff);
        }
    }

    void Update()
    {
        foreach (Transform joint in jointTransforms)
        {
            if (joint == null) continue;

            // 원래 조인트의 위치 가져오기
            Vector3 rawPosition = joint.position;

            // OneEuroFilter로 위치 필터링
            Vector3 filteredPosition = jointFilters[joint].Filter(rawPosition, Time.time);

            // 필터링된 위치 적용
            joint.position = filteredPosition;
        }
    }
}