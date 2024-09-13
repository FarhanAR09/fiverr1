using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace CoreAttack
{
    [RequireComponent(typeof(CAMovementController))]
    public class EnemyAIController : MonoBehaviour
    {
        //Movement
        [SerializeField]
        private SOStateMachine movementStateMachine;
        private CoreAttack.EnemyMovementState enemyMovementState;
        private CAMovementController movementController;
        private bool isMoving = false;
        private Vector2 targetPosition;

        //Behavior
        [SerializeField]
        private SOStateMachine behaviorStateMachine;
        [SerializeField]
        private CoreAttack.EnemyBehaviorState enemyBehaviorState;

        private void Awake()
        {
            TryGetComponent(out movementController);
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
    }
}
