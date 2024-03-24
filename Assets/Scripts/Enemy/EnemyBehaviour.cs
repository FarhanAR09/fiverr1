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

    private void Awake()
    {
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, initialPosition, initialDirection);
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
        }
    }

    private void FixedUpdate()
    {
        //Consider input every frame
        if (finishedMoving && gridMover != null && PlayerMovement.GOInstance)
        {
            if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
            {
                Vector2Int target = MapHandler.Instance.MapGrid.GetXY(PlayerMovement.GOInstance.transform.position);

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
    }

    private void OnDisable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.RemoveListener(FinishedMoving);
            gridMover.OnStartedMoving.RemoveListener(StartedMoving);
        }
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

    private void StartedMoving()
    {
        finishedMoving = false;
    }

    private void FinishedMoving()
    {
        finishedMoving = true;
    }
}
