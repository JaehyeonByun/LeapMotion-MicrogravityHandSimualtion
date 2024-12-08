using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilteredHandTracking : MonoBehaviour
{
    public Transform handTransform; // Leap Motion 손 모델의 Transform

    private OneEuroFilter<Vector3> positionFilter;

    // OneEuroFilter 파라미터 설정
    public float freq = 90.0f;       // 업데이트 주파수 (Leap Motion은 약 90Hz)
    public float minCutoff = 1.0f;   // 최소 컷오프 주파수
    public float beta = 0.007f;      // 속도에 따른 반응도 조정
    public float dCutoff = 1.0f;     // 변화율에 대한 컷오프 주파수

    void Start()
    {
        // OneEuroFilter 초기화
        positionFilter = new OneEuroFilter<Vector3>(freq, minCutoff, beta, dCutoff);
    }

    void Update()
    {
        // Leap Motion 손 모델의 원래 위치 가져오기
        Vector3 rawPosition = handTransform.position;

        // OneEuroFilter로 위치 필터링
        Vector3 filteredPosition = positionFilter.Filter(rawPosition, Time.time);

        // 필터링된 위치 적용
        handTransform.position = filteredPosition;
    }
}
