using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyBehaviourState
{
    Patrolling,
    Chasing
}

[RequireComponent(typeof(EnemyPatrol))]
public class EnemyBehaviour : MonoBehaviour, IStunnable, IPurgable
{
    [SerializeField]
    private float speed = 2.2f;
    [SerializeField]
    private Vector2Int initialPosition = new(-1 ,-1);
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    private bool finishedMoving = true;

    private GridMover gridMover;

    private EnemyPatrol enemyPatrol;
    private EnemyBehaviourState seekState = EnemyBehaviourState.Patrolling;

    private Coroutine stunCoroutine;

    private bool isStunned = false;

    [SerializeField]
    private AudioClip detectPlayerSFX, stunnedSFX;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private ParticleSystem psAbsorb;
    private float absorbInitialRate;

    private Animator animator;

    private bool playerLost = false;

    private bool isRespawning = false;
    private bool inPurge = false;

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

        //IEnumerator SetupApprotiately()
        //{
        //    yield return new WaitUntil(() => initialPosition.x > -1 && initialPosition.y > -1);
        //    gridMover = new GameObject(name + " Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        //    gridMover.transform.parent = transform;
        //    gridMover.SetUp(transform, speed, initialPosition, initialDirection);
        //}
        //StartCoroutine(SetupApprotiately());

        enemyPatrol = GetComponent<EnemyPatrol>();

        TryGetComponent(out animator);
    }

    private void Start()
    {
        UpdateVisual(Color.cyan);

        //Particle System
        if (psAbsorb != null)
        {
            absorbInitialRate = psAbsorb.emission.rateOverTime.constant;
        }
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
        }
        enemyPatrol.OnPlayerDetected.AddListener(EnterChaseStateTemporarily);

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
        }
        enemyPatrol.OnPlayerDetected.RemoveListener(EnterChaseStateTemporarily);

        GameEvents.OnPlayerLose.Remove(HandleLosing);
        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
    }

    private void FixedUpdate()
    {
        //Consider input every frame
        if (finishedMoving && gridMover != null)
        {
            if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
            {
                Vector2Int target = seekState switch
                {
                    EnemyBehaviourState.Patrolling => TryGetRandomTilePosition(), //TODO: Optimize this, but good enough (detect if target is reached)
                    EnemyBehaviourState.Chasing => PlayerInput.GOInstance != null ?
                        MapHandler.Instance.MapGrid.GetXY(PlayerInput.GOInstance.transform.position) :
                        new Vector2Int(-1, -1),
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
                directionDistances.Remove(DirectionUtils.Vector2IntToMovementDirection(-DirectionUtils.MovementDirectionToVector2Int(gridMover.InputDirection)));

                //Magic minimum search
                KeyValuePair<MovementDirection, float> minimalDirectionPair = directionDistances.Aggregate((l, r) => l.Value < r.Value ? l : r);

                if (float.IsPositiveInfinity(minimalDirectionPair.Value)) //Default to reversing
                {
                    Vector2Int reversedDirection = -DirectionUtils.MovementDirectionToVector2Int(gridMover.InputDirection);
                    gridMover.InputDirection = DirectionUtils.Vector2IntToMovementDirection(reversedDirection);
                }
                else //Pick least distance
                {
                    gridMover.InputDirection = minimalDirectionPair.Key;
                }
            }

            if (spriteRenderer != null)
                spriteRenderer.transform.localRotation = gridMover.InputDirection == MovementDirection.Left ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            if (animator != null)
            {
                if (gridMover.InputDirection == MovementDirection.Left || gridMover.InputDirection == MovementDirection.Right)
                {
                    if (seekState == EnemyBehaviourState.Patrolling)
                        animator.Play("enemy_idle", -1);
                    else
                        animator.Play("enemy_chase", -1);

                }
                else if (gridMover.InputDirection == MovementDirection.Up)
                {
                    if (seekState == EnemyBehaviourState.Patrolling)
                        animator.Play("enemy_idle_up", -1);
                    else
                        animator.Play("enemy_chase_up", -1);
                }
                else
                {
                    if (seekState == EnemyBehaviourState.Patrolling)
                        animator.Play("enemy_idle_down", -1);
                    else
                        animator.Play("enemy_chase_down", -1);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isStunned && !isRespawning && collision.TryGetComponent(out IEnemyHurtable hurtable))
        {
            if (PlaySceneManager.Instance != null)
            {
                //GameEvents.OnPlayerLose.Publish(false);
                hurtable.TryHurt();
            }
        }
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
        if (isStunned || isRespawning)
            return;

        IEnumerator HandleChaseState()
        {
            if (detectPlayerSFX != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(detectPlayerSFX, 15000);
            }

            seekState = EnemyBehaviourState.Chasing;
            UpdateVisual(Color.red);

            yield return new WaitForSeconds(8);

            seekState = EnemyBehaviourState.Patrolling;
            UpdateVisual(Color.cyan);
        }
        StopCoroutine(HandleChaseState());
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

    public void Stun(float duration)
    {
        if (playerLost || isRespawning)
            return;

        if (gridMover != null)
        {
            //Restarts stun
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);
                gridMover.Enabled = true;
                isStunned = false;
            }
            IEnumerator StunBehavior()
            {
                if (stunnedSFX != null && SFXController.Instance != null)
                {
                    SFXController.Instance.RequestPlay(stunnedSFX, 15001);
                }

                gridMover.Enabled = false;
                isStunned = true;
                yield return new WaitForSeconds(duration);
                gridMover.Enabled = true;
                isStunned = false;
            }
            stunCoroutine = StartCoroutine(StunBehavior());
        }
    }

    private void HandleLosing(bool _)
    {
        if (playerLost)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (gridMover != null)
        {
            gridMover.SetActiveState(false);
        }

        playerLost = true;
    }

    private void UpdateVisual(Color color)
    {
        if (spriteRenderer != null)
        {
            //spriteRenderer.color = color;
            spriteRenderer.material.SetColor("_Color", color);
        }

        if (psAbsorb != null)
        {
            ParticleSystem.MainModule main = psAbsorb.main;
            main.startColor = color;
        }
    }

    private void DisableByPurge(bool _)
    {
        inPurge = true;

        if (playerLost)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        //gridMover.Enabled = false;
        gridMover.SetActiveState(false);
        isStunned = true;

        if (animator != null)
            animator.Play("enemy_disappear", -1);

        if (psAbsorb != null)
            psAbsorb.Stop();
    }

    private void EnableByPurge(bool _)
    {
        inPurge = false;

        if (playerLost)
            return;

        //gridMover.Enabled = true;
        gridMover.SetActiveState(true);
        isStunned = false;

        if (animator != null)
            animator.Play("enemy_spawn", -1);

        if (psAbsorb != null)
            psAbsorb.Play();
    }

    public bool TryPurge()
    {
        if (isRespawning)
            return false;
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        if (animator != null)
            animator.Play("enemy_disappear", -1);
        StopCoroutine(Respawn());
        StartCoroutine(Respawn());
        return true;
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        if (gridMover != null)
        {
            gridMover.ForceMoveTo(initialPosition);
            //gridMover.Enabled = false;
            gridMover.SetActiveState(false);
        }

        if (psAbsorb != null)
        {
            var emission = psAbsorb.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(absorbInitialRate * 4);
        }

        yield return new WaitForSeconds(12f);
        yield return new WaitUntil(() => !inPurge);

        isRespawning = false;
        if (gridMover != null)
        {
            //gridMover.Enabled = true;
            gridMover.SetActiveState(true);
        }

        if (animator != null)
            animator.Play("enemy_spawn", -1);

        if (psAbsorb != null)
        {
            var emission = psAbsorb.emission;
            emission.rateOverTime = new ParticleSystem.MinMaxCurve(absorbInitialRate);
        }
    }
}