using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    [RequireComponent(typeof(CAMovementController))]
    public class EnemyAIController : MonoBehaviour
    {
        //Movement
        [SerializeField]
        private SOStateMachine movementStateMachine;
        [SerializeField]
        private CoreAttack.EnemyMovementState enemyMovementState;
        private CAMovementController movementController;
        private bool isMoving = false;
        private Vector2 targetPosition = Vector2.zero;

        //Behavior
        [SerializeField]
        private SOStateMachine behaviorStateMachine;
        [SerializeField]
        private CoreAttack.EnemyBehaviorState enemyBehaviorState;

        //Contact Damager
        [SerializeField]
        private ContactDamager contactDamager;

        private void Awake()
        {
            TryGetComponent(out movementController);

            //FSM Setup
            if (!movementStateMachine.Equals(behaviorStateMachine))
            {
                if (enemyMovementState != null)
                {
                    enemyMovementState = Instantiate(enemyMovementState);
                    enemyMovementState.Setup(this, movementStateMachine);
                    if (movementStateMachine != null)
                    {
                        movementStateMachine.ChangeState(enemyMovementState);
                    }
                    else Debug.LogWarning("No Movement State Machine in " + name);
                }
                if (enemyBehaviorState != null)
                {
                    enemyBehaviorState = Instantiate(enemyBehaviorState);
                    enemyBehaviorState.Setup(this, behaviorStateMachine);
                    if (behaviorStateMachine != null)
                    {
                        behaviorStateMachine.ChangeState(enemyBehaviorState);
                    }
                    else Debug.LogWarning("No Behavior State Machine in " + name);
                }
            }
            else Debug.LogWarning("movementStateMachine and behaviorStateMachine is the same component.");
        }

        private void FixedUpdate()
        {
            //Movement
            if (isMoving && movementController != null)
            {
                movementController.MoveTo(targetPosition);
            }
        }

        public void SetMovementEnable(bool on)
        {
            isMoving = on;
        }

        public void SetMovementTarget(Vector2 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public void SetContactDamagerEnabled(bool on)
        {
            if (contactDamager != null)
            {
                contactDamager.ContactDamagerEnabled = on;
            }
            else Debug.LogWarning("ContactDamager is null in " + name);
        }
    }
}
