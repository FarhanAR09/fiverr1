using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyBehaviourState
{
    Patrolling,
    Chasing
}

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.2f;
    private GridMover gridMover;

    private void Awake()
    {
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, new Vector2Int(0, 0));
    }

    //TODO: implement enemy
}
