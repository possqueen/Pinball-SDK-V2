using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Actions/Homing")]
public class HomingState : PlayerState
{
    [HideInInspector] public Transform CurrentTarget;
    Transform activeTarget;
    public HomingReticle reticle;
    public AudioClip HomingSound;
    [Tooltip("The layer that Sonic will be targeting")]
    public LayerMask HomingLayer;
    [Tooltip ("The layers that block homing targeting. Sonic won't be able to target an object if a collider on this layer/s is between them.")]
    public LayerMask BlockingLayers;
    [Tooltip("Maximum radius for acquiring homing targets")]
    public float MaxSearchRadius;
    [Tooltip("Field of view for targeting objects, based on what direction Sonic is currently facing.")]
    [Range(-1, 1)] public float FieldOfView;
    [Tooltip("Speed of homing attacks")]
    public float BaseHomingSpeed;
    [Tooltip("Speed of air dashes")]
    public float BaseDashSpeed;
    [Tooltip("Rate for smoothing out the velocity redirection")]
    public float VelocitySmoothing;
    [Tooltip("Duration of air dashes, in seconds.")]
    public float DashDuration;
    [Tooltip("Timeout for if you happen to get caught on a ledge or wall while homing an object")]
    public float HomingTimeout;
    [Tooltip("Multiplier added to velocity upon exiting air dashes")]
    [Range(0, 1)] public float DashReleaseVelocity;
    [Tooltip("Multiplier added to velocity upon exiting homing attacks")]
    [Range(0, 1)] public float HomingReleaseVelocity;
    [Tooltip("Bounce height granted when homing objects")]
    public float HomingReleaseBounce;
    [Tooltip("If false, Sonic will only be able to target objects that are on screen.")]
    public bool TargetOffscreenObjects;
    bool IsAirDash;
    float t;
    Vector3 Direction;
    Vector3 TargetVelocity;
    float DashSpeed;
    float HomingSpeed;

    public override void Initialize(PlayerController p, PlayerActions a)
    {
        base.Initialize(p, a);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        IsAirDash = CurrentTarget == null;
        player.UseGravity = false;
        actions.actionSound.PlayOneShot(HomingSound);
        if (IsAirDash)
        {
            Direction = player.MoveInput.magnitude > 0.1f ? player.MoveInput : transform.forward;
        }
        t = 0;
        DashSpeed = player.Velocity.magnitude > BaseDashSpeed ? player.Velocity.magnitude : BaseDashSpeed;
        HomingSpeed = player.Velocity.magnitude > BaseHomingSpeed ? player.Velocity.magnitude : BaseHomingSpeed;
    }

    public override void OnUpdate()
    {
        t += Time.deltaTime;
        if (IsAirDash)
        {
            TargetVelocity = Direction * DashSpeed;
            if (t >= DashDuration)
            {
                player.Velocity *= DashReleaseVelocity;
                actions.GetState<DefaultState>(out DefaultState d);
                d.CanDash = false;
                actions.ChangeState<DefaultState>();
            }
        } else
        {
            Direction = CurrentTarget.position - transform.position;
            TargetVelocity = Direction.normalized * HomingSpeed;
            if (t >= HomingTimeout)
            {
                actions.ChangeState<DefaultState>();
            }
        }
    }

    public override void OnFixedUpdate()
    {
        if (IsAirDash)
        {
            if (t < DashDuration)
            {
                //player.Velocity = Direction.normalized * DashSpeed;
                player.Velocity = TargetVelocity;
            }
        } else
        {
            //player.Velocity = Direction.normalized * HomingSpeed;
            player.Velocity = Vector3.Lerp(player.Velocity, TargetVelocity, VelocitySmoothing * Time.fixedDeltaTime);
        }
    }


    public override void OnExit()
    {
        base.OnExit();
        player.UseGravity = true;
    }

    private void Update()
    {
        if (!player.Grounded)
        {
            activeTarget = ClosestTarget(HomingLayer, MaxSearchRadius, FieldOfView);
            if (activeTarget != null)
            {
                if (CurrentTarget != activeTarget)
                {
                    reticle.Active = true;
                    CurrentTarget = activeTarget;
                }
                Debug.DrawLine(transform.position, activeTarget.position, Color.green);
                
            } else
            {
                CurrentTarget = null;
                reticle.Active = false;
            }
            if (CurrentTarget != null)
            {
                reticle.SetReticle(CurrentTarget.position);
            }
        } else
        {
            reticle.Active = false;
            activeTarget = null;
            CurrentTarget = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerActions.currentState == this)
        {
            if (other.TryGetComponent<EnemyHealth>(out EnemyHealth e))
            {
                e.DealDamage();
                player.Velocity *= HomingReleaseVelocity;
                player.Velocity.y = HomingReleaseBounce;
                actions.GetState<DefaultState>(out DefaultState d);
                d.CanDash = true;
                actions.ChangeState<DefaultState>();
            }
        }
    }

    public Transform ClosestTarget(LayerMask layer, float Radius, float FOV)
    {
        RaycastHit[] TargetsInRange = Physics.SphereCastAll(transform.position, Radius, transform.forward, Radius, layer);
        Transform closestTarget = null;
        float distance = 1f;
        foreach (RaycastHit t in TargetsInRange)
        {
            Transform target = t.transform;
            Vector3 Direction = target.position - transform.position;
            bool Facing = Vector3.Dot(Direction.normalized, transform.forward) > FOV;
            float TargetDistance = (Direction.sqrMagnitude / Radius) / Radius;
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (TargetDistance < distance && Facing && (onScreen && !TargetOffscreenObjects || TargetOffscreenObjects))
            {
                if (!Physics.Linecast(transform.position, target.position, BlockingLayers))
                {
                    closestTarget = target;
                    distance = TargetDistance;
                }
            }
        }

        return closestTarget;
    }

    private void OnDrawGizmos()
    {
        //Debug the field of view
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(90f * -FieldOfView, transform.up) * transform.right * MaxSearchRadius, Color.blue);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(90f * FieldOfView, transform.up) * -transform.right * MaxSearchRadius, Color.blue);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, MaxSearchRadius);
    }
}
