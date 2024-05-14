using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

public class BitsEaterBehaviour : MonoBehaviour, IStunnable, IPurgable, IScoreCollector
{
    //Grid Mover Properties
    [SerializeField]
    private float speed = 2.2f;
    [SerializeField]
    private Vector2Int initialPosition = new(-1, -1);
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    private GridMover gridMover;
    //private bool finishedMoving = true;

    //Set Up
    private bool beenSetUp = false;
    private float storedSpeed;
    private Vector2Int storedInitialPosition;
    private MovementDirection storedInitialDirection;

    private Transform targetTransform = null;
    
    private bool playerLost = false;
    private bool isRespawning = false;
    //private bool inPurge = false;

    private Animator animator;
    [SerializeField]
    private ParticleSystem psEatBit, psSpawning;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    //Score Collection
    public bool CanEatUncorrupted { get; } = true;
    public bool CanEatCorrupted { get; } = false;
    public bool EatingBitProduceScore { get; } = false;
    public bool CanCorruptBit { get; } = true;

    private void Awake()
    {
        if (beenSetUp)
        {
            speed = storedSpeed;
            initialPosition = storedInitialPosition;
            initialDirection = storedInitialDirection;
        }

        //gridMover = new GameObject(name + " Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        //gridMover.transform.parent = transform;
        //gridMover.SetUp(transform, speed, initialPosition, initialDirection);

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
        //    Debug.Log("Setup!");
        //}
        //StartCoroutine(SetupApprotiately());

        TryGetComponent(out animator);

        if (spriteRenderer != null)
        {
            spriteRenderer.material.SetFloat("_Intensity", 4);
            spriteRenderer.material.SetColor("_Color", Color.red);
        }

        if (psSpawning != null)
        {
            var emission = psSpawning.emission;
            emission.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
            gridMover.OnFinishedMoving.AddListener(DetermineDirection);
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
            gridMover.OnFinishedMoving.RemoveListener(DetermineDirection);
        }
        GameEvents.OnPlayerLose.Remove(HandleLosing);
        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
    }

    private void StartedMoving()
    {
        //finishedMoving = false;
    }

    private void FinishedMoving()
    {
        //finishedMoving = true;
    }

    private void DetermineDirection()
    {
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            //Determining target
            ScorePellet targetBit = UncorruptedCacheTracker.Instance != null ? UncorruptedCacheTracker.Instance.TryGetBit() : null;
            targetTransform = targetBit != null ? targetBit.transform : null;
            Vector2Int target = targetTransform != null ?
                MapHandler.Instance.MapGrid.GetXY(targetTransform.position) :
                TryGetRandomTilePosition();

            //Direction and its distance to target
            Dictionary<MovementDirection, float> directionDistances = new()
                {
                    { MovementDirection.Up, CalculateDistanceSqr(gridMover.Position + Vector2Int.up, target) },
                    { MovementDirection.Down, CalculateDistanceSqr(gridMover.Position + Vector2Int.down, target) },
                    { MovementDirection.Left, CalculateDistanceSqr(gridMover.Position + Vector2Int.left, target) },
                    { MovementDirection.Right, CalculateDistanceSqr(gridMover.Position + Vector2Int.right, target) }
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

    private void HandleLosing(bool _)
    {
        if (playerLost)
            return;

        if (gridMover != null)
        {
            gridMover.SetActiveState(false);
        }

        playerLost = true;
    }

    /// <summary>
    /// Modified random position so that failed return can still be used.
    /// </summary>
    /// <returns>If fails, returns origin</returns>
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
        return new Vector2Int(0, 0);
    }

    public void Stun(float duration)
    {
        if (isRespawning)
            return;
        StopCoroutine(Respawn());
        StartCoroutine(Respawn());
    }

    public bool TryPurge()
    {
        if (isRespawning)
            return false;
        StopCoroutine(Respawn());
        StartCoroutine(Respawn());
        return true;
    }

    private IEnumerator Respawn()
    {
        //isRespawning = true;
        //if (animator != null)
        //    animator.Play("enemy_disappear", -1);
        //if (gridMover != null)
        //{
        //    gridMover.ForceMoveTo(initialPosition);
        //    //gridMover.Enabled = false;
        //    gridMover.SetActiveState(false);
        //}
        //if (psSpawning != null)
        //{
        //    var emission = psSpawning.emission;
        //    emission.enabled = true;
        //}

        //yield return new WaitForSeconds(12f);
        //yield return new WaitUntil(() => !inPurge);

        //isRespawning = false;
        //if (gridMover != null)
        //{
        //    //gridmover.enabled = true;
        //    gridMover.SetActiveState(true);
        //}

        //if (animator != null)
        //    animator.Play("enemy_spawn", -1);

        //if (psSpawning != null)
        //{
        //    var emission = psSpawning.emission;
        //    emission.enabled = false;
        //}

        float animDur = 0;
        if (animator != null)
        {
            animator.Play("enemy_disappear", -1);
            animDur = animator.GetCurrentAnimatorStateInfo(0).length;
        }
        yield return new WaitForSeconds(animDur);
        Destroy(gameObject);
    }

    private void DisableByPurge(bool _)
    {
        //inPurge = true;

        if (playerLost)
            return;

        //gridMover.Enabled = false;
        gridMover.SetActiveState(false);

        if (animator != null)
            animator.Play("enemy_disappear", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Stop();
    }

    private void EnableByPurge(bool _)
    {
        //inPurge = false;

        if (playerLost)
            return;

        //gridMover.Enabled = true;
        gridMover.SetActiveState(true);

        if (animator != null)
            animator.Play("enemy_spawn", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Play();
    }

    public void NotifyBitEaten()
    {
        if (psEatBit != null)
        {
            psEatBit.Emit(8);
        }
    }

    public void Setup(float speed, Vector2Int initialPosition, MovementDirection initialDirection)
    {
        storedSpeed = speed;
        storedInitialPosition = initialPosition;
        storedInitialDirection = initialDirection;
        beenSetUp = true;
    }
}
