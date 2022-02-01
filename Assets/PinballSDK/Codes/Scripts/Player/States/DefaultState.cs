using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Actions/Default")]
public class DefaultState : PlayerState
{
    [HideInInspector] public bool CanDash;
    public bool ToggleRoll; //If true, Sonic will toggle between running and rolling when the button is pressed. If false, Sonic will only roll while the button is pressed.
    public float RollingStartSpeed = 5f;
    public override void Initialize(PlayerController p, PlayerActions a)
    {
        base.Initialize(p, a);
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        if (player.Grounded)
        {
            CanDash = true;
            if (input.GetButton("Jump", InputManager.InputBehavior.Down))
            {
                actions.ChangeState<JumpState>();
            }

            if (player.Velocity.magnitude >= RollingStartSpeed)
            {
                if (ToggleRoll)
                {
                    if (Input.GetButtonDown("Roll"))
                    {

                    }
                }
            }

        } else
        {
            if (input.GetButton("Jump", InputManager.InputBehavior.Down) && CanDash)
            {
                actions.ChangeState<HomingState>();
            }
        }
    }
}
