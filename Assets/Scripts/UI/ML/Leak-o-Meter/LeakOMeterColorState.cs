using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeakOMeterColorState : State
{
    public Color Color { get; private set; }
    private SpriteRenderer glowRenderer;

    //Transition
    private readonly float transDur = 2f;
    private float transTime = 0f;

    public LeakOMeterColorState(GameObject owner, StateMachine stateMachine, Color color, SpriteRenderer glowRenderer) : base(owner, stateMachine) {
        Color = color;
        this.glowRenderer = glowRenderer;
    }

    public override void Enter()
    {
        base.Enter();
        
        transTime = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (glowRenderer != null && transTime < transDur)
        {
            glowRenderer.color = Color.Lerp(glowRenderer.color, Color, 5f * Time.unscaledDeltaTime);
            transTime += Time.unscaledDeltaTime;
        }
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
