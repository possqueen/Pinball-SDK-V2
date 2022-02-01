using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("")]
public class PlayerState : MonoBehaviour
{
    [HideInInspector] public PlayerController player;
    [HideInInspector] public PlayerActions actions;
    [HideInInspector] public InputManager input;
    [Tooltip("Index of this state, so the Animator knows what state we should be in.")] public int StateIndex;
    /// <summary>
    /// Used for initializing the state on start, to set up certain values that won't need to be set later.
    /// </summary>
    public virtual void Initialize(PlayerController p, PlayerActions a)
    {
        player = p;
        actions = a;
        input = InputManager.instance;
    }
    /// <summary>
    /// Called when entering a state. This can be used for things such as initial jump forces or setting up a spin dash.
    /// </summary>
    public virtual void OnEnter()
    {
        actions._anim._anim.SetInteger("State", StateIndex);
    }

    /// <summary>
    /// This state's update loop. Called from Update in the action controller, but can be used in tandem with a normal Update function for background processes.
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// This state's fixed update loop. Called from FixedUpdate in the action controller, but can be used in tandem with a normal FixedUpdate function for background processes.
    /// </summary>
    public virtual void OnFixedUpdate() { }
    /// <summary>
    /// Called when exiting the state and transitioning to another. This can be used for things such as exiting rails.
    /// </summary>
    public virtual void OnExit() { }
}
