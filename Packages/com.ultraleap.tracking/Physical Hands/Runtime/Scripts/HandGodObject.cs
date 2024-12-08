using UnityEngine;
using System.Collections.Generic;
namespace Leap.PhysicalHands
{
    public class HandGodObject : MonoBehaviour
    { 
     [Header("Tracked Hand Joints")]
        [Tooltip("Manually assign tracked hand joints.")]
        public Transform trackedThumb;
        public Transform trackedIndexFinger;
        public Transform trackedMiddleFinger;
        public Transform trackedRingFinger;
        public Transform trackedPinky;
        public Transform trackedPalm;

        [Header("Rendered Hand Joints")]
        [Tooltip("Manually assign rendered hand joints.")]
        public Transform renderedThumb;
        public Transform renderedIndexFinger;
        public Transform renderedMiddleFinger;
        public Transform renderedRingFinger;
        public Transform renderedPinky;
        public Transform renderedPalm;

        private Dictionary<Transform, Transform> jointMappings = new Dictionary<Transform, Transform>();

        private void Start()
        {
            // 매핑 초기화
            InitializeMappings();
        }

        private void Update()
        {
            // 매프레임 렌더링 손 업데이트
            UpdateRenderedHand();
        }

        private void InitializeMappings()
        {
            // 각 관절을 매핑
            AddMapping(trackedThumb, renderedThumb, "Thumb");
            AddMapping(trackedIndexFinger, renderedIndexFinger, "Index Finger");
            AddMapping(trackedMiddleFinger, renderedMiddleFinger, "Middle Finger");
            AddMapping(trackedRingFinger, renderedRingFinger, "Ring Finger");
            AddMapping(trackedPinky, renderedPinky, "Pinky");
            AddMapping(trackedPalm, renderedPalm, "Palm");
        }

        private void AddMapping(Transform trackedJoint, Transform renderedJoint, string jointName)
        {
            if (trackedJoint == null || renderedJoint == null)
            {
                Debug.LogWarning($"Joint mapping for {jointName} is incomplete. Please assign both tracked and rendered joints.");
                return;
            }

            if (!jointMappings.ContainsKey(trackedJoint))
            {
                jointMappings.Add(trackedJoint, renderedJoint);
            }
        }

        private void UpdateRenderedHand()
        {
            foreach (var mapping in jointMappings)
            {
                Transform trackedJoint = mapping.Key;
                Transform renderedJoint = mapping.Value;

                // 트래킹 관절의 위치와 회전을 렌더링 관절에 동기화
                renderedJoint.position = trackedJoint.position;
                renderedJoint.rotation = trackedJoint.rotation;
            }
        }
    }
}
