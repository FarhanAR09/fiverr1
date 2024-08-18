using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPeekedState : CardState
{
    public CardPeekedState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) { }

    private readonly float peekDuration = 2f;
    private float peekTime = 0f;

    public override void Enter()
    {
        base.Enter();
        if (Owner != null)
        {
            Owner.SetReveal(true);
            if (Owner.Corrupted)
                Owner.SetBackgroundColor(Color.red);
        }
        peekTime = peekDuration;
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
        peekTime -= Time.fixedDeltaTime;
        if (peekTime <= 0f)
        {
            if (Owner != null && Owner.DownState != null && stateMachine != null)
            {
                stateMachine.ChangeState(Owner.DownState);
            }
        }
    }
}
