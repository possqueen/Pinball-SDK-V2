using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Actions/Jump")]
public class JumpState : PlayerState
{
    public AudioClip jumpSound;
    public float HopForce;
    public float JumpSpeed;
    public float MinJumpDuration;
    public float MaxJumpDuration;
    float t;
    public override void Initialize(PlayerController p, PlayerActions a)
    {
        base.Initialize(p, a);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        t = 0;
        player.transform.position += player.GroundNormal * (player.GroundCheckDist + 0.01f); //Move Sonic above the ground slightly so he is no longer grounded
        player.Velocity += player.GroundNormal * HopForce; //Add the initial jump value
        actions.actionSound.PlayOneShot(jumpSound);
    }

    public override void OnUpdate()
    {
        if (t < MaxJumpDuration)
        {
            t += Time.deltaTime;
        }
        if (input.GetButton("Jump", InputManager.InputBehavior.Up))
        {
            if (t > MinJumpDuration) t = MaxJumpDuration;
        }
        if (player.Grounded && t >= MinJumpDuration)
        {
            actions.ChangeState<DefaultState>();
        }

        if (input.GetButton("Jump", InputManager.InputBehavior.Down))
        {
            actions.ChangeState<HomingState>();
        }
    }

    public override void OnFixedUpdate()
    {
        if (t < MaxJumpDuration)
        {
            player.Velocity += player.GroundNormal * JumpSpeed * Time.fixedDeltaTime;
        }

        Vector3 PlanarVelocity = Vector3.ProjectOnPlane(player.Velocity, player.GroundNormal);
        Vector3 AirVelocity = player.Velocity - PlanarVelocity;

        float Drag = 1f - (player.AirDrag * Time.fixedDeltaTime);
        PlanarVelocity *= Drag;
        player.Velocity = PlanarVelocity + AirVelocity;
    }
}
