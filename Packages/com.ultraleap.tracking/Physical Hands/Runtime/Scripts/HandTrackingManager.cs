using UnityEngine;
using Leap;

public class HandTrackingManager : MonoBehaviour
{
    Controller leapController;

    void Start()
    {
        leapController = new Controller();
    }

    void Update()
    {
        Frame frame = leapController.Frame();
        foreach (Hand hand in frame.Hands)
        {
            Debug.Log("Hand detected: " + (hand.IsLeft ? "Left" : "Right"));
            Debug.Log("Palm Position: " + hand.PalmPosition);
            Debug.Log("Palm Normal: " + hand.PalmNormal);
        }
    }
}
