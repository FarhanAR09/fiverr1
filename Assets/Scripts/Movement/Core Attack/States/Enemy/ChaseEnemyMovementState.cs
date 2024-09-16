using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreAttack;

namespace CoreAttack
{
    [CreateAssetMenu(fileName = "New Chase Enemy Movement State", menuName = "States/Core Attack/Enemy Movement State/Chase")]
    public class ChaseEnemyMovementState : EnemyMovementState
    {
        public override void Enter()
        {
            base.Enter();
            if (OwnerAI != null)
            {
                OwnerAI.SetMovementEnable(true);
            }
            else Debug.LogWarning("Owner is null in Chase Enemy Movement State Start");
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

            if (OwnerAI != null)
            {
                if (Player.Instance != null)
                {
                    OwnerAI.SetMovementTarget(Player.Instance.transform.position);
                }
            }
            else Debug.LogWarning("Owner is null in Chase Enemy Movement State Physics Update");
        }

        public override void Setup(EnemyAIController ownerAI, SOStateMachine stateMachine)
        {
            base.Setup(ownerAI, stateMachine);
        }
    }
}