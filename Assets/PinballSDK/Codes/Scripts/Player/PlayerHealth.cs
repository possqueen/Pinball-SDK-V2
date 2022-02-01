using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerController player;
    public int RingAmount;
    public AudioSource ringSource;
    public AudioClip LoseRings;
    public int MaxLostRings = 30;
    [Tooltip ("Makes it so you keep some rings after a certain threshold when dropping them, much like modern games do.")]
    public bool KeepRingsOnHit;
    public float RingLossRadius;
    public float RingLossForce;
    public float RingLossHeight;
    ObjectPool pool;
    bool pitchBend;

    private void Start()
    {
        pool = ObjectPool.PoolManager;
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!ringSource.isPlaying)
        {
            pitchBend = false;
            ringSource.pitch = 1f;
        }
        if (RingAmount > 0)
        {
            if (Input.GetKeyDown("r"))
            {
                DropRings();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<RingActor>(out RingActor ring))
        {
            if (ring.collectible)
            {
                RingAmount += ring.Value;
                pool.SpawnFromPool("RingParticle", ring.transform.position, Quaternion.identity);
                RingSound();
                other.gameObject.SetActive(false);
            }
        }
    }

    public void RingSound()
    {
        if (ringSource.isPlaying)
        {
            pitchBend = !pitchBend;
            ringSource.pitch = pitchBend ? 0.99f : 1f;
        }
        ringSource.Play();
    }

    public void DropRings()
    {
        int amount = RingAmount;
        if (RingAmount >= MaxLostRings)
        {
            amount = MaxLostRings;
        }
        if (KeepRingsOnHit)
        {
            RingAmount -= amount;
        } else
        {
            RingAmount = 0;
        }
        ringSource.pitch = 1f;
        ringSource.PlayOneShot(LoseRings);
        switch (player.currentAxis)
        {
            case PlayerController.MovementAxis.Full:
                float angle = 360f / (float)amount;
                for (int i = 0; i < amount; i++)
                {
                    Quaternion Rotation = Quaternion.AngleAxis(i * angle, Vector3.up);
                    Vector3 Direction = Rotation * transform.forward;
                    Vector3 Position = transform.position + Direction * RingLossRadius;
                    Vector3 Force = Vector3.up * RingLossHeight + Direction * RingLossForce;
                    GameObject ring = pool.SpawnFromPool("LostRing", Position, Quaternion.identity);
                    Rigidbody ring_rb = ring.GetComponent<Rigidbody>();
                    ring_rb.velocity = Vector3.zero;
                    ring_rb.AddForce(Force, ForceMode.Impulse);
                }
                break;
            case PlayerController.MovementAxis.Sidescroll:
                float Angle2D = (player.Grounded ? 180f : 360f) / (float)amount;
                for (int i = 0; i < amount; i++)
                {
                    Quaternion Rotation = Quaternion.AngleAxis(i * Angle2D, player.scrollAxis.Z_Axis);
                    Vector3 Direction = Rotation * Quaternion.FromToRotation (player.scrollAxis.Y_Axis, player.GroundNormal) * player.scrollAxis.X_Axis;
                    Vector3 Position = transform.position + Direction * RingLossRadius;
                    Vector3 Force = Direction * RingLossHeight;
                    GameObject ring = pool.SpawnFromPool("LostRing", Position, Quaternion.identity);
                    Rigidbody ring_rb = ring.GetComponent<Rigidbody>();
                    ring_rb.velocity = Vector3.zero;
                    ring_rb.AddForce(Force, ForceMode.Impulse);
                }
                break;
        }
    }
}
