using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingSound : StateMachineBehaviour
{
    public AudioClip landing;
    AudioSource source;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (!source) source = animator.GetComponent<FootstepHandler>().FootstepSource;
        source.PlayOneShot(landing);
    }
}
