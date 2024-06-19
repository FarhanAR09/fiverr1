using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LowLevel;

public class TrojanBehaviour : MonoBehaviour
{
    private GridMover gridMover;
    private bool finishedMoving = true, playerLost = false, inPurge = false;

    private MovementDirection currentDirection = MovementDirection.Right;

    [SerializeField]
    private float speed = 2.2f;
    [SerializeField]
    private Vector2Int initialPosition = new(-1, -1);
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;

    private void Awake()
    {
        gridMover = new GameObject(name + " Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            gridMover.SetUp(transform, speed, MapHandler.Instance.MapGrid.GetXY(transform.position), initialDirection);
        }
        else
        {
            gridMover.SetUp(transform, speed, initialPosition, initialDirection);
        }
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
            gridMover.OnStartedMoving.AddListener(UpdateLaneDetectionPosition);
        }

        GameEvents.OnPlayerLose.Add(HandleLosing);
        GameEvents.OnPurgeWarning.Add(DisableByPurge);
        GameEvents.OnPurgeFinished.Add(EnableByPurge);
    }

    private void OnDisable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.RemoveListener(FinishedMoving);
            gridMover.OnStartedMoving.RemoveListener(StartedMoving);
            gridMover.OnStartedMoving.RemoveListener(UpdateLaneDetectionPosition);
        }

        GameEvents.OnPlayerLose.Remove(HandleLosing);
        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
    }

    private void FixedUpdate()
    {
        if (finishedMoving && gridMover != null)
        {
            if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
            {
                Vector2Int target = TryGetRandomTilePosition(); //TODO: Optimize this, but good enough (detect if target is reached); 
                Dictionary<MovementDirection, float> directionDistances = new()
                {
                    { MovementDirection.Up, CalculateDistanceSqr(gridMover.Position + new Vector2Int(0, 1), target) },
                    { MovementDirection.Down, CalculateDistanceSqr(gridMover.Position + new Vector2Int(0, -1), target) },
                    { MovementDirection.Left, CalculateDistanceSqr(gridMover.Position + new Vector2Int(-1, 0), target) },
                    { MovementDirection.Right, CalculateDistanceSqr(gridMover.Position + new Vector2Int(1, 0), target) }
                };

                //Remove reverse from checking
                directionDistances.Remove(DirectionUtils.Vector2IntToMovementDirection(-DirectionUtils.MovementDirectionToVector2Int(gridMover.InputDirection)));

                //Magic minimum search
                KeyValuePair<MovementDirection, float> minimalDirectionPair = directionDistances.Aggregate((l, r) => l.Value < r.Value ? l : r);

                if (float.IsPositiveInfinity(minimalDirectionPair.Value)) //Default to reversing
                {
                    Vector2Int reversedDirection = -DirectionUtils.MovementDirectionToVector2Int(gridMover.InputDirection);
                    currentDirection = DirectionUtils.Vector2IntToMovementDirection(reversedDirection);
                }
                else //Pick least distance
                {
                    currentDirection = minimalDirectionPair.Key;
                }
                gridMover.InputDirection = currentDirection;
            }
        }
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

    private void HandleLosing(bool enabled)
    {
        if (playerLost)
            return;

        //if (stunCoroutine != null)
        //    StopCoroutine(stunCoroutine);

        if (gridMover != null)
        {
            gridMover.Enabled = enabled;
        }

        playerLost = true;
    }

    private void DisableByPurge(bool _)
    {
        inPurge = true;

        if (playerLost)
            return;

        //if (stunCoroutine != null)
        //    StopCoroutine(stunCoroutine);
        //gridMover.Enabled = false;
        gridMover.SetActiveState(false);
        //isStunned = true;

        //if (animator != null)
        //    animator.Play("enemy_disappear", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Stop();
    }

    private void EnableByPurge(bool _)
    {
        inPurge = false;

        if (playerLost)
            return;

        //gridMover.Enabled = true;
        gridMover.SetActiveState(true);
        //isStunned = false;

        //if (animator != null)
        //    animator.Play("enemy_spawn", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Play();
    }

    private void UpdateLaneDetectionPosition()
    {
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            MapHandler.Instance.MapGrid.GetXY(transform.position, out int x, out int y);
            Vector2Int initCheckGridPos = new Vector2Int(x, y) + DirectionUtils.MovementDirectionToVector2Int(currentDirection);
            Debug.Log(initCheckGridPos);
            Debug.DrawRay(
                MapHandler.Instance.MapGrid.GetWorldPosition(initCheckGridPos.x, initCheckGridPos.y) +
                MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
                Vector2.one * 2,
                Color.red,
                2f);

            //Element is lane starting cell
            List<Vector2Int> verticalLane = new(), horizontalLane = new();
            bool CheckCell(Vector2Int gridPos, Vector2Int direction)
            {
                if (MapHandler.Instance.MapGrid.GetGridObject(gridPos.x, gridPos.y).Walkable)
                {
                    CheckCell(gridPos + direction, direction);
                    return true;
                }
                else
                {
                    if (Mathf.Abs(direction.x) == 1)
                    {
                        horizontalLane.Add(direction);
                    }
                    else if (Mathf.Abs(direction.y) == 1)
                    {
                        verticalLane.Add(direction);
                    }
                    return false;
                }
            }
            CheckCell(initCheckGridPos + new Vector2Int(-1, 0), new Vector2Int(-1, 0));
            CheckCell(initCheckGridPos + new Vector2Int(1, 0), new Vector2Int(1, 0));
            CheckCell(initCheckGridPos + new Vector2Int(0, -1), new Vector2Int(0, -1));
            CheckCell(initCheckGridPos + new Vector2Int(0, 1), new Vector2Int(0, 1));
        }
    }
}
