using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MouthController : MonoBehaviour
{
    public Transform Head;
    public Transform MouthLeft;
    public Transform MouthRight;
    public Transform camera;
    bool IsMouthLeft;
    public bool ManuallyAssign;

    private void Start()
    {
        if (!ManuallyAssign)
        {
            MouthLeft = Head.Find("MouthReference_L");
            MouthRight = Head.Find("MouthReference");
            camera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        IsMouthLeft = Head.InverseTransformPoint(camera.position).y <= 0;
        MouthRight.localScale = IsMouthLeft ? Vector3.one : Vector3.zero;
        MouthLeft.localScale = IsMouthLeft ? Vector3.zero : Vector3.one;
    }
}
