using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    CameraMotor cam;
    public float Duration;
    public CameraState OnEnter;
    public CameraState OnExit;
    public bool DefaultOnExit;
    public bool ToggleToDefault;
    bool Toggle;

    private void Start()
    {
        cam = FindObjectOfType<CameraMotor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ToggleToDefault) Toggle = !Toggle;
            if (OnEnter)
            {
                if (ToggleToDefault)
                {
                    CameraState target = Toggle ? OnEnter : cam.DefaultState;
                    cam.ChangeState(target, Duration);
                } else
                {
                    cam.ChangeState(OnEnter, Duration);
                }
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (OnExit)
            {
                cam.ChangeState(OnExit, Duration);
            } else
            {
                if (DefaultOnExit)
                {
                    cam.ChangeState(cam.DefaultState, Duration);
                }
            }
        }
    }
}
