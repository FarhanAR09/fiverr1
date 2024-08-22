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
            Owner.SetReveal(true);
            if (Owner.Corrupted)
                Owner.SetBackgroundColor(Color.red);
            Owner.StartFlipAnimation(true);
            Owner.EmitParticles();
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
