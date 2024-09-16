using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreAttack;

namespace CoreAttack
{
    public class DefaultEnemyBehaviorState : EnemyBehaviorState
    {
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

        public override void Setup(EnemyAIController ownerAI, SOStateMachine stateMachine)
        {
            base.Setup(ownerAI, stateMachine);
        }
    }
}