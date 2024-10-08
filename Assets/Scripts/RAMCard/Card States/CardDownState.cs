using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
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
            Owner.OnHoverDown += EnableHighlight;
            Owner.OnHoverUp += DisableHighlight;
        }
        GameEvents.OnMLFlashPowerStarted.Add(PeekCard);

        GameEvents.OnMLAllCardsPaired.Add(RemoveCardIfCorrupt);
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (Owner != null)
        {
            Owner.OnClicked -= OnClicked;
            Owner.OnHoverDown -= EnableHighlight;
            Owner.OnHoverUp -= DisableHighlight;
        }
        GameEvents.OnMLFlashPowerStarted.Remove(PeekCard);

        GameEvents.OnMLAllCardsPaired.Remove(RemoveCardIfCorrupt);
    }

    public override void Enter()
    {
        base.Enter();

        if (Owner != null)
        {
            Owner.SetReveal(false);
            Owner.SetBackgroundColor(Color.white);
            Owner.StartFlipAnimation(false);
            Owner.EmitSFXCardFlip();
        }

        GameEvents.OnMLCardFlipped.Publish(new CardFlipArgument(Owner, false));
    }

    public override void Exit()
    {
        base.Exit();

        DisableHighlight();
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
            stateMachine.ChangeState(Owner.UpState);
        }
    }

    private void PeekCard(bool _)
    {
        if (Owner != null && Owner.PeekedState != null && stateMachine != null)
        {
            stateMachine.ChangeState(Owner.PeekedState);
        }
    }

    private void RemoveCardIfCorrupt(bool _)
    {
        if (Owner != null && Owner.Corrupted)
        {
            Owner.gameObject.SetActive(false);
        }
    }

    private void EnableHighlight()
    {
        if (Owner != null)
        {
            Owner.GlowHighlight(true);
        }
    }

    private void DisableHighlight()
    {
        if (Owner != null)
        {
            Owner.GlowHighlight(false);
        }
    }
}
