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
    private bool finishedMoving = true, playerLost = false, inPurge = false, isAttacking = false, isDestroying = false;
    //private bool beenSetUp = false;

    private MovementDirection currentDirection = MovementDirection.Right;

    [SerializeField]
    private float speed = 2.2f, attackingSpeed = 6f;
    [SerializeField]
    private Vector2Int initialPosition = new(-1, -1);
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;

    //Lane Detection
    private LaneDetector vLaneDetector, hLaneDetector;

    //Animation
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    [SerializeField]
    private ParticleSystem psPlayerDetected, psCharging, psHitWall;

    //SFX
    [SerializeField]
    private AudioClip detectPlayerSFX, hitSFX;

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

        TryGetComponent(out animator);

        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            spriteRenderer.material.SetColor("_Color", Color.cyan);
        }
    }

    private void Start()
    {
        ChargeEmittingParticle(false);
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
            gridMover.OnStartedMoving.AddListener(UpdateLaneDetectionPosition);
            gridMover.OnDeniedMoving.AddListener(DestroyWhenHittingWall);
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
            gridMover.OnDeniedMoving.RemoveListener(DestroyWhenHittingWall);
        }

        GameEvents.OnPlayerLose.Remove(HandleLosing);
        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
    }

    private void FixedUpdate()
    {
        if (finishedMoving && gridMover != null && !isAttacking)
        {
            if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
            {
                Vector2Int target = TryGetRandomTilePosition(); //TODO: Optimize this, but good enough (detect if target is reached before checking)
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

        if (spriteRenderer != null && gridMover != null)
        {
            if (gridMover.CurrentDirection == MovementDirection.Left)
            {
                spriteRenderer.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (gridMover.CurrentDirection == MovementDirection.Right)
            {
                spriteRenderer.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inPurge && collision.TryGetComponent(out IEnemyHurtable hurtable))
        {
            if (hurtable.TryHurt())
            {
                if (hitSFX != null && SFXController.Instance != null)
                {
                    SFXController.Instance.RequestPlay(hitSFX, 15000);
                }
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

    private void HandleLosing(bool _)
    {
        if (playerLost)
            return;

        //if (stunCoroutine != null)
        //    StopCoroutine(stunCoroutine);

        if (gridMover != null)
        {
            gridMover.SetActiveState(false);
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

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

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

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        //isStunned = false;

        //if (animator != null)
        //    animator.Play("enemy_spawn", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Play();
    }

    private void UpdateLaneDetectionPosition()
    {
        if (vLaneDetector != null)
        {
            Destroy(vLaneDetector.gameObject);
        }
        if (hLaneDetector != null)
        {
            Destroy(hLaneDetector.gameObject);
        }
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            MapHandler.Instance.MapGrid.GetXY(transform.position, out int x, out int y);
            Vector2Int initCheckGridPos = new Vector2Int(x, y) + DirectionUtils.MovementDirectionToVector2Int(currentDirection);
            //Debug.Log(initCheckGridPos);
            //Debug.DrawRay(
            //    MapHandler.Instance.MapGrid.GetWorldPosition(initCheckGridPos.x, initCheckGridPos.y) +
            //    MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
            //    Vector2.one * 2,
            //    Color.red,
            //    2f);

            //Element is lane starting cell
            List<Vector2Int> verticalLaneCellPoss = new(), horizontalLaneCellPoss = new();
            //Check if gridPos is walkable or not recursively in direction
            bool CheckCellWalkable(Vector2Int gridPos, Vector2Int direction)
            {
                if (
                    MapHandler.Instance.MapGrid.GetGridObject(gridPos.x, gridPos.y) != null &&
                    MapHandler.Instance.MapGrid.GetGridObject(gridPos.x, gridPos.y).Walkable
                )
                {
                    bool nextCheckWalkable = CheckCellWalkable(gridPos + direction, direction);
                    if (!nextCheckWalkable)
                    {
                        if (Mathf.Abs(direction.x) == 1)
                        {
                            horizontalLaneCellPoss.Add(gridPos);
                        }
                        else if (Mathf.Abs(direction.y) == 1)
                        {
                            verticalLaneCellPoss.Add(gridPos);
                        }
                        //Debug.DrawRay(
                        //    MapHandler.Instance.MapGrid.GetWorldPosition(gridPos.x, gridPos.y) +
                        //    MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
                        //    new Vector2(-1, 1) * 2,
                        //    Color.green,
                        //    2f
                        //);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            CheckCellWalkable(initCheckGridPos, new Vector2Int(-1, 0));
            CheckCellWalkable(initCheckGridPos, new Vector2Int(1, 0));
            CheckCellWalkable(initCheckGridPos, new Vector2Int(0, -1));
            CheckCellWalkable(initCheckGridPos, new Vector2Int(0, 1));

            if (verticalLaneCellPoss.Count == 2 && horizontalLaneCellPoss.Count == 2)
            {
                //Debug.Log(verticalLane[0] + ", " + verticalLane[1]);
                //Debug.Log(horizontalLane[0] + ", " + horizontalLane[1]);
                //Debug.DrawLine(
                //    MapHandler.Instance.MapGrid.GetWorldPosition(verticalLane[0].x, verticalLane[0].y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
                //    MapHandler.Instance.MapGrid.GetWorldPosition(verticalLane[1].x, verticalLane[1].y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one, 
                //    Color.magenta, 3f);
                //Debug.DrawLine(
                //    MapHandler.Instance.MapGrid.GetWorldPosition(horizontalLane[0].x, horizontalLane[0].y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
                //    MapHandler.Instance.MapGrid.GetWorldPosition(horizontalLane[1].x, horizontalLane[1].y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one,
                //    Color.magenta, 3f);
                vLaneDetector = new GameObject("Vertical Lane Detector", typeof(LaneDetector)).GetComponent<LaneDetector>();
                vLaneDetector.Setup(
                    MapHandler.Instance.MapGrid.GetWorldPosition(verticalLaneCellPoss[0]),
                    MapHandler.Instance.MapGrid.GetWorldPosition(verticalLaneCellPoss[1]),
                    MapHandler.Instance.MapGrid.GetCellSize(), true);
                hLaneDetector = new GameObject("Horizontal Lane Detector", typeof(LaneDetector)).GetComponent<LaneDetector>();
                hLaneDetector.Setup(
                    MapHandler.Instance.MapGrid.GetWorldPosition(horizontalLaneCellPoss[0]),
                    MapHandler.Instance.MapGrid.GetWorldPosition(horizontalLaneCellPoss[1]),
                    MapHandler.Instance.MapGrid.GetCellSize(), false);
                vLaneDetector.OnPlayerDetected += AttackLane;
                hLaneDetector.OnPlayerDetected += AttackLane;
            }
        }
    }

    private void AttackLane(LaneDetectionData ldd)
    {
        //Debug.Log($"Player detected on {ldd.IsVertical}: {ldd.DetectedGO.name}");
        if (!isAttacking && !inPurge && gridMover != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            Vector2Int attackDirection = ldd.GridPos - MapHandler.Instance.MapGrid.GetXY(transform.position);
            if (attackDirection.x != 0 ^ attackDirection.y != 0)
            {
                if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
                {
                    if (Mathf.Sign(attackDirection.x) > 0)
                    {
                        attackDirection = Vector2Int.right;
                    }
                    else
                    {
                        attackDirection = Vector2Int.left;
                    }
                }
                else
                {
                    if (Mathf.Sign(attackDirection.y) > 0)
                    {
                        attackDirection = Vector2Int.up;
                    }
                    else
                    {
                        attackDirection = Vector2Int.down;
                    }
                }
                isAttacking = true;
                IEnumerator AngryThenCharge()
                {
                    //if (sr != null)
                    //{
                    //    sr.color = Color.red;
                    //}
                    if (animator != null)
                    {
                        animator.Play("trojan_angry", -1);
                    }
                    if (spriteRenderer != null && spriteRenderer.material != null)
                    {
                        spriteRenderer.material.SetColor("_Color", Color.red);
                        spriteRenderer.material.SetFloat("_Intensity", 3f);
                    }
                    if (psPlayerDetected != null)
                    {
                        psPlayerDetected.Emit(10);
                    }
                    if (detectPlayerSFX != null && SFXController.Instance != null)
                    {
                        SFXController.Instance.RequestPlay(detectPlayerSFX, 15000);
                    }

                    if (gridMover.Enabled)
                    {
                        yield return new WaitForSeconds(0.5f);

                        if (gridMover != null)
                        {
                            gridMover.Speed = attackingSpeed;
                            gridMover.ForceToDirection(DirectionUtils.Vector2IntToMovementDirection(attackDirection));
                        }

                        ChargeEmittingParticle(true);
                    }
                }
                StartCoroutine(AngryThenCharge());
                //Debug.Log(attackDirection.x + " " + attackDirection.y);
            }
        }
    }

    private void DestroyWhenHittingWall()
    {
        if (isAttacking && !isDestroying)
        {
            isDestroying = true;
            if (psHitWall != null)
            {
                psHitWall.Emit(8);
            }
            ChargeEmittingParticle(false);
            if (hitSFX != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(hitSFX, 15000);
            }
            Destroy(gameObject, 1f);
        }
    }

    public void Setup(float speed, Vector2Int initialPosition, MovementDirection initialDirection)
    {
        this.speed = speed;
        //storedInitialPosition = initialPosition;
        this.initialDirection = initialDirection;
        //beenSetUp = true;
        if (gridMover != null)
        {
            gridMover.ForceToDirection(initialDirection);
        }
        if (psPlayerDetected != null)
        {
            psPlayerDetected.Emit(10);
        }
    }

    private void ChargeEmittingParticle(bool enabled)
    {
        if (psCharging != null)
        {
            var em = psCharging.emission;
            em.enabled = enabled;
        }
    }
}
