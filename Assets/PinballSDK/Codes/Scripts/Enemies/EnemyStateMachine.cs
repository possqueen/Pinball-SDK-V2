using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyState[] States;
    [HideInInspector] public EnemyState currentState;

    public void SetState<T>() where T : EnemyState
    {
        if (currentState != null) currentState.OnExit();
        EnemyState state = null;
        for (int i = 0; i < States.Length; i++)
        {
            if (States[i] is T)
            {
                state = States[i];
            }
        }

        if (state != null)
        {
            currentState = state;
            currentState.OnEnter();
        }
        else
        {
            Debug.LogError("EnemyStateMachine: The given state \"" + typeof(T).ToString() + "\" was not found.");
        }
    }
    private void Start()
    {
        foreach (EnemyState s in States)
        {
            s.Initialize();
        }
    }
    #region Update Functions
    private void Update()
    {
        currentState.OnUpdate();
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    private void LateUpdate()
    {
        currentState.OnLateUpdate();
    }
    #endregion
    #region Trigger and Collision Functions
    private void OnTriggerEnter(Collider other)
    {
        currentState.OnEnterTrigger(other);
    }

    private void OnTriggerStay(Collider other)
    {
        currentState.OnStayTrigger(other);
    }

    private void OnTriggerExit(Collider other)
    {
        currentState.OnExitTrigger(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentState.OnEnterCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        currentState.OnStayCollision(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        currentState.OnExitCollision(collision);
    }
    #endregion

}
