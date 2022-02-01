using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : MonoBehaviour
{
    [HideInInspector] public CameraMotor _cam;
    [HideInInspector] public Vector3 TargetPosition;
    [HideInInspector] public Vector3 TargetFollowDirection;
    [HideInInspector] public Vector3 TargetLookDirection;
    public virtual void OnEnable() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnExit() { }
}
