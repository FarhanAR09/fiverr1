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
public class EnemyBehaviour : MonoBehaviour, IStunnable
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

    private Coroutine stunCoroutine;

    private bool isStunned = false;

    [SerializeField]
    private AudioClip detectPlayerSFX, stunnedSFX;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private ParticleSystem psAbsorb;

    private Animator animator;

    private bool playerLost = false;

    private void Awake()
    {
        gridMover = new GameObject(name + " Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, initialPosition, initialDirection);

        enemyPatrol = GetComponent<EnemyPatrol>();

        TryGetComponent(out animator);
            
    }

    private void Start()
    {
        UpdateVisual(Color.cyan);
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);

            GameEvents.OnPlayerLose.Add(HandleLosing);
        }
        enemyPatrol.OnPlayerDetected.AddListener(EnterChaseStateTemporarily);

        GameEvents.OnPurgeWarning.Add(DisableByPurge);
        GameEvents.OnPurgeFinished.Add(EnableByPurge);
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
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isStunned && PlayerInput.GOInstance != null && collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            if (PlaySceneManager.Instance != null)
            {
                GameEvents.OnPlayerLose.Publish(false);
            }
        }
    }

    private void OnDisable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.RemoveListener(FinishedMoving);
            gridMover.OnStartedMoving.RemoveListener(StartedMoving);

            GameEvents.OnPlayerLose.Remove(HandleLosing);
        }
        enemyPatrol.OnPlayerDetected.RemoveListener(EnterChaseStateTemporarily);

        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
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
        if (isStunned)
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
        if (playerLost)
            return;

        if (gridMover != null)
        {
            //Restarts stun
            if (stunCoroutine != null)
            {
                StopCoroutine(stunCoroutine);

                //Stunned by outside factors? Don't restart
                if (isStunned)
                {
                    return;
                }
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

    private void HandleLosing(bool enabled)
    {
        if (playerLost)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (gridMover != null)
        {
            gridMover.Enabled = enabled;
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
        if (playerLost)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        gridMover.Enabled = false;
        isStunned = true;

        if (animator != null)
            animator.Play("enemy_disappear", -1);

        if (psAbsorb != null)
            psAbsorb.Stop();
    }

    private void EnableByPurge(bool _)
    {
        if (playerLost)
            return;

        gridMover.Enabled = true;
        isStunned = false;

        if (animator != null)
            animator.Play("enemy_spawn", -1);

        if (psAbsorb != null)
            psAbsorb.Play();
    }
}