using UnityEngine;

public class CardState : State
{
    public new RAMCard Owner { get; protected set; }

    public CardState(RAMCard owner, StateMachine stateMachine) : base(owner.gameObject, stateMachine) {
        this.Owner = owner;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}