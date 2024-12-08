using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilteredHandTracking : MonoBehaviour
{
    public Transform handTransform; // Leap Motion �� ���� Transform

    private OneEuroFilter<Vector3> positionFilter;

    // OneEuroFilter �Ķ���� ����
    public float freq = 90.0f;       // ������Ʈ ���ļ� (Leap Motion�� �� 90Hz)
    public float minCutoff = 1.0f;   // �ּ� �ƿ��� ���ļ�
    public float beta = 0.007f;      // �ӵ��� ���� ������ ����
    public float dCutoff = 1.0f;     // ��ȭ���� ���� �ƿ��� ���ļ�

    void Start()
    {
        // OneEuroFilter �ʱ�ȭ
        positionFilter = new OneEuroFilter<Vector3>(freq, minCutoff, beta, dCutoff);
    }

    void Update()
    {
        // Leap Motion �� ���� ���� ��ġ ��������
        Vector3 rawPosition = handTransform.position;

        // OneEuroFilter�� ��ġ ���͸�
        Vector3 filteredPosition = positionFilter.Filter(rawPosition, Time.time);

        // ���͸��� ��ġ ����
        handTransform.position = filteredPosition;
    }
}
