using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
    public AudioSource FootstepSource;
    public AudioClip[] Footsteps;

    public void PlayFootstep()
    {
        int RNG = Random.Range(0, Footsteps.Length);
        FootstepSource.PlayOneShot(Footsteps[RNG]);
    }
}
