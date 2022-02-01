using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Controller")]
public class PlayerController : MonoBehaviour
{
    InputManager input;
    public enum MovementAxis { Full, Sidescroll }
    [HideInInspector] public MovementAxis currentAxis;
    [HideInInspector] public SidescrollTrigger scrollAxis;
    [HideInInspector] public Rigidbody _rb;
    CapsuleCollider col;
    public LayerMask CollidesWith;
    public LayerMask WalkableLayers;
    [HideInInspector] public Vector3 Velocity;
    [HideInInspector] public Vector3 Position;
    float capsuleOffset = 0.15f;
    Collider[] collision;
    [HideInInspector] public Vector3 MoveInput;
    Transform camTransform;
    [HideInInspector] public Vector3 GroundNormal;
    [HideInInspector] public Vector3 GroundTangent;
    Vector3 TransformNormal;
    [HideInInspector] public bool Grounded;
    [HideInInspector] public float GroundHeight = 0.5f;
    Vector3 facing;
    Quaternion rotFix = Quaternion.Euler(90f, 0, 0);

    //Movement Physics
    public Vector3 Gravity;
    public Vector3 SlopeGravity;
    [Tooltip("Multiplier added to slope force while moving uphill, based on percentage of Max Speed (0-1).")]public AnimationCurve UphillForceOverSpeed;
    public float GroundCheckDist;
    [Tooltip("Multiplies the ground check distance by speed, to avoid having Sonic fly off the ground at high speeds")] [SerializeField] AnimationCurve GroundCheckOverSpeed;
    public float GroundStickSpeed; //Speed for adjusting Sonic's ground position
    public float MeshRotationSpeed;
    public float GroundNormalSmoothing;
    public float AirNormalSmoothing;
    public float MaxAngleDifference;
    public float MinAngle = 15f;

    //Movement Values
    public float MaxSpeed;
    public float TopSpeed;
    public float AccelRate;
    [Tooltip("Multiplier added to acceleration rate, based on percentage of Max Speed (0-1).")] public AnimationCurve AccelRateOverSpeed;
    public float MaxDecelRate;
    public float MinDecelRate;
    [Tooltip("Lerp value between Min Decel and Max Decel, based on percentage of Max Speed (0-1).")] public AnimationCurve DecelRateOverSpeed;
    public float Handling;
    [Tooltip("Multiplier added to handling, based on percentage of Max Speed (0-1).")] public AnimationCurve HandlingOverSpeed;
    [Tooltip("Multiplier added to handling while in air")][Range(0, 1)] public float InAirHandling;
    [Tooltip("Drag applied to slope force while running. This is not applied when rolling.")] public float RunningDrag;
    [Tooltip("Drag added while rolling on flat surfaces")] public float RollingDrag;
    [HideInInspector] public bool Crouching;
    public float BrakeForce;
    public float AirDrag;

    public float MinStickSpeed; //Minimum speed for sticking to the ground
    public float MaxStandingAngle = 90f;
    float GroundAngle;

    [HideInInspector] public bool Rolling;
    [HideInInspector] public bool InputLocked;
    [HideInInspector] public bool Braking;
    [HideInInspector] public bool UseGravity = true;
    [HideInInspector] public bool OverrideVelocity;


    private void Start()
    {
        input = InputManager.instance;
        _rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        camTransform = FindObjectOfType<CameraMotor>().transform;
        Position = _rb.position;
        GroundNormal = Vector3.up;
        facing = transform.forward;
        collision = new Collider[10];
    }

    private void Update()
    {
        if (!InputLocked)
        {
            switch (currentAxis)
            {
                case MovementAxis.Full:
                    UpdateInput();
                    break;
                case MovementAxis.Sidescroll:
                    UpdateInput2D();
                    break;
                default:
                    UpdateInput();
                    break;
            }
        }
        SolveCollision();
    }

    private void FixedUpdate()
    {
        SolveCollision();
        float groundCheck = GroundCheckDist;
        if (PlayerActions.currentState is DefaultState)
        {
            groundCheck *= GroundCheckOverSpeed.Evaluate(Velocity.magnitude / MaxSpeed);
        }
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit rayHit, GroundHeight + groundCheck, WalkableLayers))
        {
            Grounded = true;
            Vector3 groundPoint = rayHit.point + rayHit.normal * GroundHeight;
            _rb.position = Vector3.MoveTowards(_rb.position, groundPoint, GroundStickSpeed * Time.fixedDeltaTime);
            //_rb.position = rayHit.point + rayHit.normal * GroundHeight;
            GroundNormal = rayHit.normal;
            TransformNormal = Vector3.Slerp(TransformNormal, GroundNormal, Time.fixedDeltaTime * GroundNormalSmoothing);
            GroundAngle = Vector3.Angle(GroundNormal, Vector3.up);
            SlopePhysics();
        } else
        {
            Grounded = false;
            GroundNormal = -Gravity.normalized;
            TransformNormal = Vector3.Slerp(TransformNormal, GroundNormal, Time.fixedDeltaTime * AirNormalSmoothing);
            if (UseGravity)
            {
                Velocity += Gravity * Time.fixedDeltaTime;
            }
        }
        switch (currentAxis)
        {
            case MovementAxis.Full:
                HandleMovement();
                break;
            case MovementAxis.Sidescroll:
                HandleMovement();
                Velocity = Vector3.ProjectOnPlane(Velocity, scrollAxis.Z_Axis);
                break;
            default:
                HandleMovement();
                break;
        }
        //SlopePrediction(Time.fixedDeltaTime);
        if (Grounded) Velocity = Vector3.ProjectOnPlane(Velocity, GroundNormal);
        HighSpeedFix(Time.fixedDeltaTime);
        //Set rotation
        Vector3 GroundSpeed = Vector3.ProjectOnPlane(Velocity, TransformNormal);
        if (GroundSpeed.magnitude > 0.5f)
        {
            facing = Vector3.Slerp(facing, GroundSpeed, MeshRotationSpeed * Time.fixedDeltaTime);
            Quaternion facingRot = Quaternion.LookRotation(facing, TransformNormal);
            //transform.rotation = Quaternion.Slerp(transform.rotation, facing, MeshRotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(facingRot);
        }

        if (!OverrideVelocity)_rb.MovePosition(_rb.position + Velocity * Time.fixedDeltaTime);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        SolveCollision();
    }

    private void OnCollisionStay(Collision collision)
    {
        SolveCollision();
    }
    */
    private void LateUpdate()
    {
        
    }

    void HandleMovement()
    {
        Vector3 LateralVelocity = Vector3.ProjectOnPlane(Velocity, GroundNormal);
        Vector3 AirVelocity = Velocity - LateralVelocity;
        //LateralVelocity = Vector3.MoveTowards(LateralVelocity, MoveInput * DebugMoveSpeed, Time.fixedDeltaTime * DebugMoveHandling);
        Vector3 MoveVector = LateralVelocity;
        float Speed = LateralVelocity.magnitude;
        if (MoveInput.magnitude > 0.1f)
        {
            Braking = Vector3.Dot(LateralVelocity, MoveInput) < -0.5f && Speed > 0.1f && Grounded;

            if (Speed < TopSpeed && (Grounded || PlayerActions.currentState is JumpState))
            {
                float accelRate = AccelRate * AccelRateOverSpeed.Evaluate(Speed / TopSpeed);
                LateralVelocity += MoveInput * AccelRate * Time.fixedDeltaTime;
            }
            float handling = Handling * HandlingOverSpeed.Evaluate(Speed / TopSpeed) * (Grounded ? 1f : InAirHandling);
            if (!Braking)
            {
                MoveVector = Vector3.Lerp(LateralVelocity, MoveInput.normalized * Speed, handling * Time.fixedDeltaTime);
            } else
            {
                MoveVector = Vector3.MoveTowards(LateralVelocity, MoveInput.normalized * Speed, BrakeForce * Time.fixedDeltaTime);
            }
        } else
        {
            if (Braking) Braking = false;
            if (Grounded)
            {
                if (GroundAngle <= 15f)
                {
                    if (!Crouching)
                    {
                        float decelRate = Mathf.Lerp(MinDecelRate, MaxDecelRate, DecelRateOverSpeed.Evaluate(Speed / MaxSpeed));
                        if (MoveVector.magnitude > 0.5f) MoveVector = Vector3.MoveTowards(MoveVector, Vector3.zero, decelRate * Time.fixedDeltaTime);
                    } else
                    {
                        MoveVector = Vector3.MoveTowards(MoveVector, Vector3.zero, RollingDrag * Time.fixedDeltaTime);
                    }
                    if (MoveVector.magnitude < 0.5f) MoveVector = Vector3.zero;
                } else
                {
                    float handling = SlopeGravity.magnitude * HandlingOverSpeed.Evaluate(Speed / TopSpeed);
                    MoveVector = Vector3.MoveTowards(MoveVector, GroundTangent * MoveVector.magnitude, SlopeGravity.magnitude * Time.fixedDeltaTime);
                }
            } else
            {
                //Add drag in air
                float airDrag = 1f - (AirDrag * Time.fixedDeltaTime);
                if (airDrag <= 0f) airDrag = 0f;
                MoveVector *= airDrag;
            }

        }

        MoveVector = Vector3.ClampMagnitude(MoveVector, MaxSpeed);

        Velocity = MoveVector + AirVelocity;
    }

    void UpdateInput2D()
    {
        Vector3 RawInput = new Vector3(-input.GetAxis("Horizontal"), 0, 0);
        Vector3 TransformedInput = Quaternion.FromToRotation(Vector3.up, GroundNormal) * (Quaternion.Euler(scrollAxis.PlaneRotation) * RawInput);
        TransformedInput = Vector3.ProjectOnPlane(TransformedInput, GroundNormal);
        MoveInput = TransformedInput;
    }

    void UpdateInput()
    {
        float h = input.GetAxis("Horizontal");
        float v = input.GetAxis("Vertical");
        Vector3 rawInput = new Vector3(h, 0, v);
        rawInput = Vector3.ClampMagnitude(rawInput, 1f);
        Vector3 TransformedInput = Quaternion.FromToRotation(camTransform.up, GroundNormal) * (camTransform.rotation * rawInput);
        TransformedInput = Vector3.ProjectOnPlane(TransformedInput, GroundNormal);
        MoveInput = TransformedInput.normalized * rawInput.magnitude;
    }

    void SolveCollision()
    {
        Vector3 capCenter = transform.position + col.center;
        if (Rolling) capCenter = transform.position - transform.up * (GroundHeight - col.radius);
        //collision = Physics.OverlapCapsule(capCenter - transform.up * capsuleOffset, capCenter + transform.up * capsuleOffset, col.radius, CollidesWith);
        int count = Physics.OverlapSphereNonAlloc(capCenter + Velocity * Time.fixedDeltaTime, col.radius, collision, CollidesWith);
        Vector3 overlapDir;
        float overlapDist;
        //Then we go through all possible collisions with a for loop
        for (int i = 0; i < count; i++)
        {
            Collider _col = collision[i];
            Vector3 colPos = _col.transform.position;
            Quaternion colRot = _col.transform.rotation;

            //After setting up the collision data, we use ComputePenetration to figure out what direction and how far to push the character out of the colliders
            bool Colliding = Physics.ComputePenetration(col, transform.position, transform.rotation, _col, colPos, colRot, out overlapDir, out overlapDist);
            if (Colliding)
            {
                Debug.DrawRay(transform.position, overlapDir * overlapDist, Color.red);
                //Now we simply add the direction and distance to the rigidbody's position.
                _rb.position += overlapDir * overlapDist;
                Velocity = Vector3.ProjectOnPlane(Velocity, overlapDir);
            }
        }

        //Check along velocity just in case we are moving too fast for the overlap sphere to catch collision
        Ray velocityRay = new Ray(transform.position, Velocity.normalized);
        if (Physics.Raycast(velocityRay, out RaycastHit rayHit, Velocity.magnitude * Time.fixedDeltaTime, CollidesWith))
        {
            Velocity = Vector3.ProjectOnPlane(Velocity, rayHit.normal);
        }

        
    }

    private void OnDrawGizmos()
    {
        if (col)
        {
            //Draw collision
            Vector3 center = Rolling ? transform.position - transform.up * (GroundHeight - col.radius) : transform.position + col.center;
            float Radius = col.radius;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(center, Radius);
        }
    }

    /// <summary>
    /// This function serves to more or less fix Sonic's velocity and position based on a predicted position and direction,
    /// done using a series of raycasts along Sonic's current velocity by the delta time. This is so he can better stick to
    /// slopes and have less trouble traversing lower poly terrains.
    /// </summary>
    void HighSpeedFix(float dt)
    {
        Vector3 PredictedPosition = _rb.position;
        Vector3 PredictedNormal = GroundNormal;
        Vector3 PredictedVelocity = Velocity;
        int steps = 8;
        int i;
        for (i = 0; i < steps; i++)
        {
            PredictedPosition += PredictedVelocity * dt / steps;
            if (Physics.Raycast(PredictedPosition, -PredictedNormal, out RaycastHit pGround, GroundHeight + GroundCheckDist, WalkableLayers))
            {
                if (Vector3.Angle(PredictedNormal, pGround.normal) < MaxAngleDifference)
                {
                    Debug.DrawRay(PredictedPosition, -PredictedNormal, Color.green);
                    PredictedPosition = pGround.point + pGround.normal * 0.5f;
                    PredictedVelocity = Quaternion.FromToRotation(GroundNormal, pGround.normal) * PredictedVelocity;
                    PredictedNormal = pGround.normal;
                }
                else
                {
                    Debug.DrawRay(PredictedPosition, -PredictedNormal, Color.red);
                    i = -1;
                    break;
                }
            }
            else
            {
                Debug.DrawRay(PredictedPosition, -PredictedNormal, Color.red);
                i = -1;
                break;
            }
        }
        if (i >= steps)
        {
            GroundNormal = PredictedNormal;
            _rb.position = Vector3.MoveTowards(_rb.position, PredictedPosition, dt);
        }
    }

    /// <summary>
    /// By Greedy
    /// 
    /// This Function is made so that the character doesnt get clipped inside geometry giving him an extra layer of prediction and avoiding weird speed jumps due to colision solving
    /// </summary>
    void SlopePrediction(float dt)
    {
        float LowerValue = 0.43f;
        Vector3 PredictedPosition = _rb.position + (-GroundNormal * LowerValue); //reducing Height to get closer to the ground
        Vector3 PredictedNormal = GroundNormal;
        Vector3 PredictedVelocity = Velocity;
        float SpeedFrame = _rb.velocity.magnitude * dt; //actually a good idea to cache this call at the start of fixed update so you dont have to do calc everytime (the velocity.mag)
        float LerpJump = 0.015f;

        Debug.DrawRay(PredictedPosition, PredictedVelocity.normalized * SpeedFrame * 1.3f, Color.red, 5, true);
        if (!Physics.Raycast(PredictedPosition, PredictedVelocity.normalized, out RaycastHit pGround, SpeedFrame * 1.3f, WalkableLayers)) { HighSpeedFix(dt); return; } //if detects no slopes in the way go to Downforce check

        for (float lerp = LerpJump; lerp < MaxAngleDifference / 90; lerp += LerpJump) //increases lerp up until it breaks the angle limit
        {
            Debug.DrawRay(PredictedPosition, Vector3.Lerp(PredictedVelocity.normalized, GroundNormal, lerp) * SpeedFrame * 1.3f, Color.blue, 5, false);
            if (!Physics.Raycast(PredictedPosition, Vector3.Lerp(PredictedVelocity.normalized, GroundNormal, lerp), out pGround, SpeedFrame * 1.3f, WalkableLayers)) //Go until Find a suitable position above Ground
            {
                lerp += LerpJump; //add an extra one for extra hoverness
                Debug.DrawRay(PredictedPosition + (Vector3.Lerp(PredictedVelocity.normalized, GroundNormal, lerp) * SpeedFrame * 1.3f) + (Vector3.right * 0.05f), -PredictedNormal, Color.yellow, 5, false);
                Physics.Raycast(PredictedPosition + (Vector3.Lerp(PredictedVelocity.normalized, GroundNormal, lerp) * SpeedFrame * 1.3f), -PredictedNormal, out pGround, GroundHeight + GroundCheckDist, WalkableLayers); // hit the ground on the valid Position

                PredictedPosition = (PredictedPosition + Vector3.Lerp(PredictedVelocity.normalized, GroundNormal, lerp) * SpeedFrame) + (pGround.normal * LowerValue);
                PredictedVelocity = Quaternion.FromToRotation(GroundNormal, pGround.normal) * PredictedVelocity;
                GroundNormal = pGround.normal;
                _rb.position = Vector3.MoveTowards(_rb.position, PredictedPosition, dt);
                Velocity = PredictedVelocity;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<SidescrollTrigger>(out SidescrollTrigger trig))
        {
            switch (currentAxis)
            {
                case MovementAxis.Full:
                    currentAxis = MovementAxis.Sidescroll;
                    scrollAxis = trig;
                    //Set Z position
                    Vector3 TriggerRelativePosition = trig.transform.InverseTransformPoint(transform.position);
                    Vector3 ProjectedPosition = Vector3.ProjectOnPlane(TriggerRelativePosition, trig.Z_Axis);
                    transform.position = trig.transform.TransformPoint(ProjectedPosition);
                    break;
                case MovementAxis.Sidescroll:
                    scrollAxis = null;
                    currentAxis = MovementAxis.Full;
                    break;
            }
        }
    }

    void SlopePhysics()
    {
        //Get ground tangent
        GroundTangent = SurfaceTangent(GroundNormal, SlopeGravity);
        float slopeDot = Vector3.Dot(Velocity.normalized, GroundTangent);
        bool Uphill = slopeDot < 0;
        float slopeMod = UphillForceOverSpeed.Evaluate(Velocity.magnitude / MaxSpeed);

        if (GroundAngle > MinAngle)
        {
            //Basic slope force
            Vector3 SlopeForce = Vector3.ProjectOnPlane(SlopeGravity, GroundNormal);
            if (!Crouching) SlopeForce *= RunningDrag;
            Velocity += SlopeForce * (Uphill ? slopeMod : slopeDot) * Time.fixedDeltaTime;
        }

        if (Grounded)
        {
            if (Velocity.magnitude < MinStickSpeed && GroundAngle >= MaxStandingAngle)
            {
                _rb.position += GroundNormal * (GroundCheckDist + 0.01f);
                Grounded = false;
            }
        }
    }
    /// <summary>
    /// Creates a tangent vector based on a given normal and gravity vector.
    /// </summary>
    Vector3 SurfaceTangent(Vector3 normal, Vector3 gravity)
    {
        Vector3 v1 = Vector3.Cross(normal, gravity);
        Vector3 Tan = Vector3.Cross(v1, normal);
        return Tan.normalized;
    }

    IEnumerator InputLock(float duration)
    {
        InputLocked = true;
        MoveInput = Vector3.zero;
        yield return new WaitForSeconds(duration);
        InputLocked = false;
    }

    public void LockInput(float duration)
    {
        StartCoroutine(InputLock(duration));
    }

}
