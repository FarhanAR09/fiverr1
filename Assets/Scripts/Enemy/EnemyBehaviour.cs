using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum EnemyBehaviourState
{
    Patrolling,
    Chasing
}

[RequireComponent(typeof(EnemyPatrol))]
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    private float speed = 2.2f;
    [SerializeField]
    private Vector2Int initialPosition = Vector2Int.zero;
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    private bool finishedMoving = true;

    private GridMover gridMover;

    private EnemyPatrol enemyPatrol;
    private EnemyBehaviourState seekState = EnemyBehaviourState.Patrolling;

    private void Awake()
    {
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, initialPosition, initialDirection);

        enemyPatrol = GetComponent<EnemyPatrol>();
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
        }
        enemyPatrol.OnPlayerDetected.AddListener(EnterChaseStateTemporarily);
    }

    private void FixedUpdate()
    {
        //Consider input every frame
        if (finishedMoving && gridMover != null && PlayerMovement.GOInstance)
        {
            if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
            {
                var target = seekState switch
                {
                    EnemyBehaviourState.Patrolling => TryGetRandomTilePosition(), //TODO: Optimize this, but good enough (detect if target is reached)
                    EnemyBehaviourState.Chasing => MapHandler.Instance.MapGrid.GetXY(PlayerMovement.GOInstance.transform.position),
                    _ => new Vector2Int(-1, -1),
                };
                Dictionary<MovementDirection, float> directionDistances = new ()
                {
                    { MovementDirection.Up, CalculateDistanceSqr(gridMover.Position + new Vector2Int(0, 1), target) },
                    { MovementDirection.Down, CalculateDistanceSqr(gridMover.Position + new Vector2Int(0, -1), target) },
                    { MovementDirection.Left, CalculateDistanceSqr(gridMover.Position + new Vector2Int(-1, 0), target) },
                    { MovementDirection.Right, CalculateDistanceSqr(gridMover.Position + new Vector2Int(1, 0), target) }
                };

                //Remove reverse from checking
                directionDistances.Remove(DirectionUtils.Vector2IntToMovementDirection(-DirectionUtils.MovementDirectionToVector2Int(gridMover.Direction)));

                //Magic minimum search
                KeyValuePair<MovementDirection, float> minimalDirectionPair = directionDistances.Aggregate((l, r) => l.Value < r.Value ? l : r);

                if (float.IsPositiveInfinity(minimalDirectionPair.Value)) //Default to reversing
                {
                    Vector2Int reversedDirection = -DirectionUtils.MovementDirectionToVector2Int(gridMover.Direction);
                    gridMover.Direction = DirectionUtils.Vector2IntToMovementDirection(reversedDirection);
                }
                else //Pick least distance
                {
                    gridMover.Direction = minimalDirectionPair.Key;
                }
            }
        }

        //Debug.Log("Seek State: " + seekState.ToString());
    }

    private void OnDisable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.RemoveListener(FinishedMoving);
            gridMover.OnStartedMoving.RemoveListener(StartedMoving);
        }
        enemyPatrol.OnPlayerDetected.RemoveListener(EnterChaseStateTemporarily);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private float CalculateDistanceSqr(Vector2Int from, Vector2Int to)
    {
        //MapHandler check
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            //Grid and Tile check
            if (MapHandler.Instance.CheckBoundary(from) && MapHandler.Instance.CheckBoundary(to) && MapHandler.Instance.MapGrid.GetGridObject(from).Walkable)
            {
                return ((Vector2)(MapHandler.Instance.MapGrid.GetWorldPosition(to.x, to.y) - MapHandler.Instance.MapGrid.GetWorldPosition(from.x, from.y))).sqrMagnitude;
            }
        }
        return float.PositiveInfinity;
    }

    private void EnterChaseStateTemporarily()
    {
        IEnumerator HandleChaseState()
        {
            seekState = EnemyBehaviourState.Chasing;
            yield return new WaitForSeconds(8);
            seekState = EnemyBehaviourState.Patrolling;
        }
        StartCoroutine(HandleChaseState());
    }

    private void StartedMoving()
    {
        finishedMoving = false;
    }

    private void FinishedMoving()
    {
        finishedMoving = true;
    }

    private Vector2Int TryGetRandomTilePosition()
    {
        int maxTries = 200;
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            int maxWidth = MapHandler.Instance.MapGrid.GetWidth();
            int maxHeight = MapHandler.Instance.MapGrid.GetHeight();
            for (int i = 0; i < maxTries; i++)
            {
                Vector2Int randomTilePos = new(UnityEngine.Random.Range(0, maxWidth), UnityEngine.Random.Range(0, maxHeight));
                if (MapHandler.Instance.MapGrid.GetGridObject(randomTilePos).Walkable)
                    return randomTilePos;
            }
        }
        return new Vector2Int(-1, -1);
    }
}