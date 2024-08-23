using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPeekedState : CardState
{
    public CardPeekedState(RAMCard owner, StateMachine stateMachine) : base(owner, stateMachine) {
        flashLevel = PlayerPrefs.GetInt(GameConstants.MLUPGRADEFLASH, 1);
        if (!(flashLevel > 1 && flashLevel <= 4))
        {
            Debug.LogWarning("Flash/Peek Level Invalid: " + flashLevel);
        }
    }

    private int flashLevel = 1;
    private float peekDuration
    {
        get
        {
            return flashLevel switch
            {
                2 => 1.5f,
                3 => 2f,
                4 => 3f,
                _ => 0.1f
            };
        }
    }
    private float peekTime = 0f;

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
