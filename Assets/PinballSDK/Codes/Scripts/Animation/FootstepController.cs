using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : StateMachineBehaviour
{
    FootstepHandler step;
    public int[] StepFrames;
    public int LengthInFrames = 40; //Length of the running animation, in frames
    [SerializeField] float[] StepTimes;
    float CurrentTime;
    float PreviousTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        step = animator.GetComponent<FootstepHandler>();
        if (!step)
        {
            Debug.LogError("FootstepController: No Handler detected. Footsteps will not play.");
        }
        StepTimes = new float[StepFrames.Length];
        for (int i = 0; i < StepTimes.Length; i++)
        {
            StepTimes[i] = (float)StepFrames[i] / LengthInFrames;
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!step) return;
        CurrentTime = stateInfo.normalizedTime % 1.0f;
        if (PreviousTime != CurrentTime)
        {
            for (int i = 0; i < StepTimes.Length; i++)
            {
                if (CurrentTime >= StepTimes[i] && (PreviousTime <= StepTimes[i] || PreviousTime > CurrentTime))
                {
                    step.PlayFootstep();
                }
            }
        }
        PreviousTime = CurrentTime;
    }
}
