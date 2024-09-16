using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class EnemyMovementState : SOState
    {
        public EnemyAIController OwnerAI { get; protected set; }

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

        public virtual void Setup(EnemyAIController ownerAI, SOStateMachine stateMachine)
        {
            base.Setup(ownerAI.gameObject, stateMachine);
            OwnerAI = ownerAI;
        }
    }
}
