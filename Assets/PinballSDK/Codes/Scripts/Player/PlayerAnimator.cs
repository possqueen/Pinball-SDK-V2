using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Animator")]
public class PlayerAnimator : MonoBehaviour
{
    [HideInInspector] public Animator _anim;
    public PlayerController player;
    [SerializeField] float TurnSmoothing = 5f;
    [SerializeField] float TurningDeadzone = 0.1f;
    float turnAmount;
    float deltaRotation;

    private void Start()
    {
        _anim = GetComponent<Animator>();

    }

    private void Update()
    {
        //Set basic non-state related values
        _anim.SetBool("Grounded", player.Grounded);
        float gSpeed = Vector3.ProjectOnPlane(player.Velocity, player.GroundNormal).magnitude;
        float aSpeed = player.Velocity.y;
        _anim.SetFloat("GroundSpeed", gSpeed);
        _anim.SetFloat("AirSpeed", aSpeed);
        _anim.SetBool("Brake", player.Braking);
        deltaRotation = Vector3.Dot(transform.right, player.Velocity.normalized) * 10f;
        deltaRotation = Mathf.Clamp(deltaRotation, -1f, 1f);
        bool Turning = deltaRotation > TurningDeadzone || deltaRotation < -TurningDeadzone;
        if (Turning)
        {
            turnAmount = Mathf.Lerp(turnAmount, deltaRotation, Time.deltaTime * TurnSmoothing);
        } else
        {
            turnAmount = Mathf.Lerp(turnAmount, 0f, Time.deltaTime * TurnSmoothing);
        }
        _anim.SetFloat("YRotation", turnAmount);
    }
}
