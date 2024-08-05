using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public GameObject Owner { get; protected set; }
    protected StateMachine stateMachine;

    public State(GameObject owner, StateMachine stateMachine)
    {
        this.Owner = owner;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
}
