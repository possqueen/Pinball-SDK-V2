using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class SpringActor : MonoBehaviour
{
    AudioSource sound;
    public float SpringForce;
    public float LockDuration;
    public bool IsAdditive;

    //Debug values
    [SerializeField] bool DrawForce;
    [SerializeField] float DebugDrag;
    [SerializeField] [Range(0.1f, 10)] float DebugLength;
    [SerializeField] float DebugThickness = 0.1f;
    [SerializeField] Vector3 DebugGravity;
    [SerializeField] Color DebugColor;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (Application.isEditor)
        {
            if (DrawForce)
            {
                Vector3[] DebugTrajectoryPoints = PreviewTrajectory(transform.position + transform.up * 0.11f, transform.up * SpringForce, DebugGravity, DebugDrag, DebugLength);
                for (int i = 1; i < DebugTrajectoryPoints.Length; i++)
                {
                    Handles.color = DebugColor;
                    
                    Handles.DrawLine(DebugTrajectoryPoints[i - 1], DebugTrajectoryPoints[i], DebugThickness);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && other.gameObject.TryGetComponent<PlayerActions>(out PlayerActions actions))
        {
            if (LockDuration > 0)
            {
                player.LockInput(LockDuration);
            }
            player.Grounded = false;
            Vector3 pos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            player._rb.position = pos + transform.up * (player.GroundCheckDist + 0.05f);
            if (IsAdditive)
            {
                player.Velocity += transform.up * SpringForce;
            } else
            {
                player.Velocity = transform.up * SpringForce;
            }
            actions.ChangeState<DefaultState>();
            sound.Play();
        }
    }

    static Vector3[] PreviewTrajectory(Vector3 position, Vector3 velocity, Vector3 gravity, float drag, float time)
    {
        float timeStep = Time.fixedDeltaTime;
        int iterations = Mathf.CeilToInt(time / timeStep);
        if (iterations < 2)
        {
            Debug.LogError("PreviewTrajectory (Vector3, Vector3, Vector3, float, float): Unable to preview trajectory shorter than Time.fixedDeltaTime * 2");
            return new Vector3[0];
        }
        Vector3[] path = new Vector3[iterations];
        Vector3 pos = position;
        Vector3 vel = velocity;
        path[0] = pos;
        float dragScale = Mathf.Clamp01(1.0f - (drag * timeStep));
        for (int i = 1; i < iterations; i++)
        {
            vel = vel + (gravity * timeStep);
            vel *= dragScale;
            pos = pos + (vel * timeStep);
            path[i] = pos;
        }
        return path;
    }
}
