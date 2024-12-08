using UnityEngine;

public class GlobalGravity : MonoBehaviour
{
    void Start()
    {
        Physics.gravity = Vector3.zero;
        Debug.Log("Global gravity set to zero for microgravity simulation.");
    }
}
