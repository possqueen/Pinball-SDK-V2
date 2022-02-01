using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMotor : MonoBehaviour
{
    public Transform followTarget;
    public Transform lookTarget;
    public PlayerController player;
    CameraState previousState;
    public CameraState DefaultState;
    [SerializeField] float TransitionDamping;
    [HideInInspector] public CameraState currentState;
    public bool Transitioning;
    float transDuration;
    float transTimer;
    float lerpVal;
    Vector3 PosVelocity;
    Vector3 From;
    Vector3 To;
    Vector3 Target;

    private void Start()
    {
        currentState = DefaultState;
    }

    private void Update()
    {
        currentState.OnUpdate();
        if (Transitioning)
        {
            To = currentState.TargetPosition;
            transTimer += Time.deltaTime;
            lerpVal = transTimer / transDuration;
            Target = Vector3.Lerp(Target, To, lerpVal);
            if (transTimer >= transDuration)
            {
                Transitioning = false;
            }
        }
    }
    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        currentState.OnLateUpdate();
        if (Transitioning)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Target, ref PosVelocity, TransitionDamping);
        }
    }

    public void ChangeState(CameraState state, float duration)
    {
        Transitioning = true;
        previousState = currentState;
        currentState = state;
        Target = previousState.TargetPosition;
        transDuration = duration;
        transTimer = 0;

    }

}
