using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class CardUpState : CardState
{
    public CardUpState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) { }

    private float autoFlipTime = 0f;

    public override void OnEnable()
    {
        base.OnEnable();

        //Debug.Log(Owner.name + " subscribing to PairRequestCalled");
        if (Owner != null)
        {
            Owner.PairRequestCalled += PairCard;
            Owner.PutDownRequestCalled += PutDownCard;
            Owner.OnClicked += PutDownCardSingleCheck;
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (Owner != null)
        {
            Owner.PairRequestCalled -= PairCard;
            Owner.PutDownRequestCalled -= PutDownCard;
            Owner.OnClicked -= PutDownCardSingleCheck;
        }
    }

    public override void Enter()
    {
        base.Enter();

        //Debug.Log("Entered Up State");
        autoFlipTime = 0f;
        if (Owner != null)
        {
            Owner.SetReveal(true);
            if (Owner.Corrupted)
                Owner.SetBackgroundColor(Color.red);
            Owner.StartFlipAnimation(true);
        }

        GameEvents.OnMLCardFlipped.Publish(new CardFlipArgument(Owner, true));
    }

    public override void Exit()
    {
        base.Exit();

        if (Owner != null)
        {
            Owner.SetReveal(false);
        }
        GameEvents.OnMLCardExitUpState.Publish(Owner);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        autoFlipTime += Time.fixedDeltaTime;
        if (autoFlipTime >= (MLPlayManager.Instance != null ? MLPlayManager.Instance.CardAutoFlipDuration : 1.7f))
        {
            PutDownCardSingleCheck();
        }
    }

    private void PairCard()
    {
        if (stateMachine != null && Owner != null && Owner.PairedState != null)
        {
            stateMachine.ChangeState(Owner.PairedState);
        }
    }

    private void PutDownCard()
    {
        if (stateMachine != null && Owner != null && Owner.DownState != null)
        {
            stateMachine.ChangeState(Owner.DownState);
        }
    }

    private void PutDownCardSingleCheck()
    {
        if (stateMachine != null && Owner != null && Owner.DownState != null)
        {
            stateMachine.ChangeState(Owner.DownState);
            GameEvents.OnMLCardFinishedSingleCheck.Publish(Owner);
        }
    }
}
