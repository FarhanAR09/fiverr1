using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOStateMachine : MonoBehaviour
{
    public SOState CurrentState { get; private set; }

    public void ChangeState(SOState newState)
    {
        if (CurrentState != null)
        {
            CurrentState.OnDisable();
            CurrentState.Exit();
        }
        CurrentState = newState;
        if (CurrentState != null)
        {
            CurrentState.Enter();
            CurrentState.OnEnable();
        }
    }

    private void OnEnable()
    {
        if (CurrentState != null)
        {
            CurrentState.OnEnable();
        }
    }

    private void OnDisable()
    {
        if (CurrentState != null)
        {
            CurrentState.OnDisable();
        }
    }

    private void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.FrameUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.PhysicsUpdate();
        }
    }
}
