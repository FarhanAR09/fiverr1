using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CardDownState : CardState
{
    public CardDownState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) { }

    public override void OnEnable()
    {
        base.OnEnable();

        if (Owner != null)
        {
            //Debug.Log(Owner.name + " subscribed");
            Owner.OnClicked += OnClicked;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (Owner != null)
        {
            Owner.OnClicked -= OnClicked;
        }
    }

    public override void Enter()
    {
        base.Enter();

        if (Owner != null)
        {
            Owner.NumberRevealed(false);
        }

        GameEvents.OnCardFlipped.Publish(new CardFlipArgument(Owner, false));
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

    private void OnClicked()
    {
        if (stateMachine != null && Owner != null && Owner.UpState != null)
        {
            //Debug.Log(Owner.name + " state called");
            stateMachine.ChangeState(Owner.UpState);
        }
    }
}
