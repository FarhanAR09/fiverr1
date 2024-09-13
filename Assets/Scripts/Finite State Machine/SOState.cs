using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class SOState : ScriptableObject
{
    public GameObject Owner { get; protected set; }
    protected StateMachine stateMachine;

    public virtual void Setup(GameObject owner, StateMachine stateMachine)
    {
        Owner = owner;
        this.stateMachine = stateMachine;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
}
