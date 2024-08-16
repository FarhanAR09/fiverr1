using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPairedState : CardState
{
    public CardPairedState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) { }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Enter()
    {
        base.Enter();

        if (Owner != null)
        {
            Owner.NumberRevealed(true);
            if (Owner.Corrupted)
                Owner.SetColor(Color.red);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
