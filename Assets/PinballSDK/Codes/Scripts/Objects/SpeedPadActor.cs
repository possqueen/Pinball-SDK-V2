using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPadActor : MonoBehaviour
{
    AudioSource source;
    public float Speed;
    public float LockDuration;
    public bool SnapToPosition;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (SnapToPosition)
            {
                Vector3 pos = transform.InverseTransformPoint(player.transform.position);
                pos.x = 0;
                player.transform.position = transform.TransformPoint(pos);
            }
            player.Velocity = transform.forward * Speed;
            if (LockDuration > 0)
            {
                player.LockInput(LockDuration);
            }
            source.Play();
        }
    }
}
