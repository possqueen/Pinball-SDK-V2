using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FixedCam : CameraState
{
    public Vector3 Point;
    public float LookDamping;
    public bool LookAtPlayer;
    public Vector3 LookDirection;
    public float DirectionThickness = 2;
    public Color directionColor;
    Transform lTarg;

    private void Start()
    {
        _cam = FindObjectOfType<CameraMotor>();
        lTarg = _cam.lookTarget;
        TargetPosition = transform.TransformPoint(Point);
    }

    public override void OnUpdate()
    {
        Vector3 FixedDirection = Quaternion.Euler(LookDirection) * Vector3.forward;
        TargetLookDirection = LookAtPlayer ? (lTarg.position - _cam.transform.position).normalized : FixedDirection;
        TargetPosition = transform.TransformPoint(Point);
    }

    public override void OnLateUpdate()
    {
        if (!_cam.Transitioning)
        {
            _cam.transform.position = TargetPosition;
        }
        Quaternion targetRot = Quaternion.LookRotation(TargetLookDirection, Vector3.up);
            _cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, targetRot, Time.deltaTime * LookDamping);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.TransformPoint(Point), 0.5f);
        Handles.color = directionColor;
        Vector3 worldPoint = transform.TransformPoint(Point);
        Handles.DrawLine(worldPoint, worldPoint + (Quaternion.Euler(LookDirection) * Vector3.forward), DirectionThickness);
    }
}
