using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("Pinball/Player/Action Controller")]
public class PlayerActions : MonoBehaviour
{
    PlayerController player;
    public PlayerAnimator _anim;
    public AudioSource actionSound;
    public static PlayerState currentState;
    public static PlayerState previousState;
    public PlayerState[] States;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        //Set up a default state if we don't have any
        //This is to ensure that we always have a default state.
        if (States.Length < 1)
        {
            DefaultState _default;
            if (!gameObject.TryGetComponent<DefaultState>(out _default))
            {
                _default = gameObject.AddComponent<DefaultState>();
            }
            States = new PlayerState[1]
            {
                _default
            };
        }
        for (int i = 0; i < States.Length; i++)
        {
            States[i].Initialize(player, this);
        }
        ChangeState<DefaultState>();
    }

    private void Update()
    {
        currentState.OnUpdate();
    }

    private void FixedUpdate()
    {
        currentState.OnFixedUpdate();
    }

    public bool HasState<T>() where T : PlayerState
    {
        bool rtn = false;
        for (int i = 0; i < States.Length; i++)
        {
            if (States[i] is T)
            {
                rtn = true;
            }
        }
        return rtn;
    }

    public int IndexOf<T>() where T : PlayerState
    {
        int state = 0;
        if (HasState<T>())
        {
            for (int i = 0; i < States.Length; i++)
            {
                if (States[i] is T)
                {
                    state = i;
                }
            }
        }
        return state;
    }

    public void ChangeState<T>() where T : PlayerState
    {
        previousState = currentState;
        int stateIndex = 0;
        if (currentState != null) currentState.OnExit();
        if (HasState<T>())
        {
            for (int i = 0; i < States.Length; i++)
            {
                if (States[i] is T)
                {
                    stateIndex = i;
                }
            }
        } else
        {
            stateIndex = IndexOf<DefaultState>();
            Debug.LogError("The given state does not exist.", this);
        }

        currentState = States[stateIndex];
        currentState.OnEnter();

    }

    public void GetState<T>(out T state) where T : PlayerState
    {
        T _state = null;
        if (currentState != null) currentState.OnExit();
        if (HasState<T>())
        {
            for (int i = 0; i < States.Length; i++)
            {
                if (States[i] is T)
                {
                    _state = States[i] as T;
                }
            }
        }
        else
        {
            Debug.LogError("The given state does not exist.", this);
        }
        state = _state;
    }
}
