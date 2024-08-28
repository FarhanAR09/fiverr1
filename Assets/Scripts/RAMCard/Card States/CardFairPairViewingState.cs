using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFailPairViewingState : CardState
{
    public CardFailPairViewingState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) { }

    public override void OnEnable()
    {
        base.OnEnable();

        if (Owner != null)
        {
            Owner.PutDownRequestCalled += PutDownCard;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (Owner != null)
        {
            Owner.PutDownRequestCalled -= PutDownCard;
        }
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
            Owner.EmitSFXCardFlip();
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

    private void PutDownCard()
    {
        if (stateMachine != null && Owner != null && Owner.DownState != null)
        {
            stateMachine.ChangeState(Owner.DownState);
        }
    }
}
