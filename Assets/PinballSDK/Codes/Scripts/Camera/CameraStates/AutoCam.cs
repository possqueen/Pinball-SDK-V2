using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCam : CameraState
{
    public PlayerController player;
    public float MaxDistance;
    public float MaxHeight;
    public Vector3 LookOffset;
    public float PositionDamping;
    public float LookDamping;
    Vector3 PosVelocity;
    Vector3 RotVelocity;
    Transform fTarg;
    Transform lTarg;
    Vector3 FDir;
    private void Start()
    {
        _cam = FindObjectOfType<CameraMotor>();
        fTarg = _cam.followTarget;
        lTarg = _cam.lookTarget;
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        FDir = (transform.position - fTarg.position).normalized;
        TargetFollowDirection = Vector3.ProjectOnPlane(FDir, fTarg.up);
        TargetLookDirection = -FDir + LookOffset;  
    }

    public override void OnLateUpdate()
    {
        base.OnLateUpdate();
        TargetPosition = fTarg.position + TargetFollowDirection * MaxDistance + fTarg.up * MaxHeight + _cam.player.Velocity * Time.fixedDeltaTime;
        if (!_cam.Transitioning)
        {
            transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref PosVelocity, PositionDamping);
        }
        Quaternion targetRot = Quaternion.LookRotation(TargetLookDirection, fTarg.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * LookDamping);

    }
}
