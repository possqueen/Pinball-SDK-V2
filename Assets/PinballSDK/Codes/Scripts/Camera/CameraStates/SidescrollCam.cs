using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidescrollCam : CameraState
{
    public SidescrollTrigger plane;
    public float Z_Offset;
    public float Y_Offset;
    public float PositionDamping;
    public float LookDamping;
    Vector3 PosVelocity;
    Transform fTarg;

    private void Start()
    {
        _cam = FindObjectOfType<CameraMotor>();
        fTarg = _cam.followTarget;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        TargetLookDirection = -plane.Z_Axis;
    }

    public override void OnLateUpdate()
    {
        TargetPosition = fTarg.position + plane.Y_Axis * Y_Offset - plane.Z_Axis * Z_Offset;
        if (!_cam.Transitioning)
        {
            _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, TargetPosition, ref PosVelocity, PositionDamping);
        }
        Quaternion lookRot = Quaternion.LookRotation(TargetLookDirection, plane.Y_Axis);
            _cam.transform.rotation = Quaternion.Slerp(_cam.transform.rotation, lookRot, Time.deltaTime * LookDamping);
    }



}
