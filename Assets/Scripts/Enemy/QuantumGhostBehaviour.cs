using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;

public class QuantumGhostBehaviour : MonoBehaviour, IStunnable, IPurgable
{
    private GridMover gridMover;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private Vector2Int initialPosition = new(-1, -1);
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    private bool finishedMoving = true, playerLost = false, inPurge = false;

    /// <summary>
    /// teleportTotalCooldown = teleportCooldown + teleportPrepDuration + animation time
    /// </summary>
    [SerializeField]
    private float teleportTotalCooldown = 5f;
    private float teleportCooldown;
    private readonly float teleportPrepDuration = 1.25f;
    private float teleportCooldownTimer;
    private bool isTeleporting = false;
    private Coroutine teleportation;

    [SerializeField]
    private SpriteRenderer spriteRenderer, teleRingSpriteRenderer1, teleRingSpriteRenderer2;
    [SerializeField]
    private ParticleSystem psInTunelling, psOutTunelling, psTunnelingExplosion;
    private Coroutine outTunnellingEffects;
    private Animator animator;

    [SerializeField]
    private AudioClip portalOpenSFX, teleportSFX;

    private bool allowHurting = false;

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

        //teleportCooldown minimal is 0.5f
        teleportCooldown = teleportTotalCooldown - teleportPrepDuration;
        if (teleportCooldown < 0.5f)
        {
            teleportCooldown = 0.5f;
            teleportTotalCooldown = teleportCooldown + teleportPrepDuration;
        }

        teleportCooldownTimer = teleportCooldown;

        TryGetComponent(out animator);
    }

    private void Start()
    {
        if (psInTunelling != null)
        {
            var em = psInTunelling.emission;
            em.enabled = false;
        }
        if (psOutTunelling != null)
        {
            var em = psOutTunelling.emission;
            em.enabled = false;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.material.SetColor("_Color", new Color(0f, 0.5f, 1f));
        }
        if (teleRingSpriteRenderer1 != null)
        {
            teleRingSpriteRenderer1.material.SetColor("_GlowColor", new Color(0f, 0.25f, 1f));
            teleRingSpriteRenderer1.material.SetFloat("_Intensity", 10f);
        }
        if (teleRingSpriteRenderer2 != null)
        {
            teleRingSpriteRenderer2.material.SetColor("_GlowColor", new Color(1f, 0.4f, 0f));
            teleRingSpriteRenderer2.material.SetFloat("_Intensity", 10f);
        }
    }

    private void OnEnable()
    {
        if (gridMover != null)
        {
            gridMover.OnFinishedMoving.AddListener(FinishedMoving);
            gridMover.OnStartedMoving.AddListener(StartedMoving);
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
        }

        GameEvents.OnPlayerLose.Remove(HandleLosing);
        GameEvents.OnPurgeWarning.Remove(DisableByPurge);
        GameEvents.OnPurgeFinished.Remove(EnableByPurge);
    }

    private void FixedUpdate()
    {
        #region Wandering Movement Input
        if (finishedMoving && !inPurge && gridMover != null)
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

                MovementDirection pickedDirection;
                if (float.IsPositiveInfinity(minimalDirectionPair.Value)) //Default to reversing
                {
                    Vector2Int reversedDirection = -DirectionUtils.MovementDirectionToVector2Int(gridMover.InputDirection);
                    pickedDirection = DirectionUtils.Vector2IntToMovementDirection(reversedDirection);
                }
                else //Pick least distance
                {
                    pickedDirection = minimalDirectionPair.Key;
                }
                gridMover.InputDirection = pickedDirection;
            }
        }
        #endregion

        #region Teleport Cooldown
        if (!isTeleporting)
        {
            if (teleportCooldownTimer > 0f)
            {
                teleportCooldownTimer -= Time.fixedDeltaTime;
                //Debug.Log("Teleporting in " + teleportCooldownTimer);
            }
            else
            {
                isTeleporting = true;
                teleportCooldownTimer = teleportTotalCooldown;

                IEnumerator Teleport()
                {
                    //Debug.Log("Teleporting....................");

                    Vector2 teleportEndWorldPos = new(-1, -1);
                    if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
                    {
                        Vector2Int currentGridPos = MapHandler.Instance.MapGrid.GetXY(transform.position);
                        Vector2Int randomGridPos;
                        do
                        {
                            randomGridPos = TryGetRandomTilePosition();
                        }
                        while (Mathf.Abs(randomGridPos.x - currentGridPos.x) <= 5 && Mathf.Abs(randomGridPos.y - currentGridPos.y) <= 5);
                        teleportEndWorldPos =
                            MapHandler.Instance.MapGrid.GetWorldPosition(randomGridPos) +
                            MapHandler.Instance.MapGrid.GetCellSize() / 2f * (Vector3) Vector2.one;
                    }

                    if (psInTunelling != null)
                    {
                        var em = psInTunelling.emission;
                        em.enabled = true;
                    }
                    if (teleportEndWorldPos != new Vector2(-1, -1))
                    {
                        IEnumerator OutTunnellingEffects()
                        {
                            if (psOutTunelling != null)
                            {
                                var em = psOutTunelling.emission;
                                em.enabled = true;
                            }
                            while (true)
                            {
                                if (psOutTunelling != null)
                                {
                                    psOutTunelling.transform.position = teleportEndWorldPos;
                                }
                                yield return new WaitForFixedUpdate();
                            }
                        }
                        if (outTunnellingEffects != null)
                            StopCoroutine(outTunnellingEffects);
                        outTunnellingEffects = StartCoroutine(OutTunnellingEffects());
                    }
                    if (portalOpenSFX != null && SFXController.Instance != null)
                    {
                        SFXController.Instance.RequestPlay(portalOpenSFX, 15000);
                    }

                    yield return new WaitForSeconds(1.25f);

                    if (psInTunelling != null)
                    {
                        var em = psInTunelling.emission;
                        em.enabled = false;
                    }
                    //Scale down animation (0.25 second)
                    if (animator != null)
                    {
                        animator.Play("qg_scaledown", -1);
                    }
                    if (teleRingSpriteRenderer1 != null)
                    {
                        teleRingSpriteRenderer1.enabled = false;
                    }
                    if (teleRingSpriteRenderer2 != null)
                    {
                        teleRingSpriteRenderer2.enabled = false;
                    }

                    yield return new WaitForSeconds(0.25f);
                    yield return new WaitUntil(() => !inPurge);

                    gridMover.SetActiveState(false);
                    if (teleportEndWorldPos != new Vector2(-1, -1))
                        transform.position = teleportEndWorldPos;
                    gridMover.SetActiveState(true);

                    if (psOutTunelling != null)
                    {
                        var em = psOutTunelling.emission;
                        em.enabled = false;
                    }
                    if (psTunnelingExplosion != null)
                    {
                        psTunnelingExplosion.Emit(20);
                    }
                    if (animator != null)
                    {
                        animator.Play("qg_scaleup", -1);
                    }
                    if (teleRingSpriteRenderer1 != null)
                    {
                        teleRingSpriteRenderer1.enabled = true;
                    }
                    if (teleRingSpriteRenderer2 != null)
                    {
                        teleRingSpriteRenderer2.enabled = true;
                    }
                    if (teleportSFX != null && SFXController.Instance != null)
                    {
                        SFXController.Instance.RequestPlay(teleportSFX, 15000);
                    }

                    //Direction is set automatically?!?!?!?
                    //HOW?!?!

                    isTeleporting = false;
                }
                if (teleportation != null)
                    StopCoroutine(teleportation);
                teleportation = StartCoroutine(Teleport());
            }
        }
        #endregion
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (allowHurting && collision.TryGetComponent(out IEnemyHurtable hurtable))
        {
            //GameEvents.OnPlayerLose.Publish(true);
            hurtable.TryHurt();
        }
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
        int maxTries = 500;
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

        if (animator != null)
        {
            animator.Play("qg_scaledown", -1);
        }
        if (teleRingSpriteRenderer1 != null)
        {
            teleRingSpriteRenderer1.enabled = false;
        }
        if (teleRingSpriteRenderer2 != null)
        {
            teleRingSpriteRenderer2.enabled = false;
        }

        allowHurting = false;
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
        
        if (animator != null)
        {
            animator.Play("qg_scaleup", -1);
        }
        if (teleRingSpriteRenderer1 != null)
        {
            teleRingSpriteRenderer1.enabled = true;
        }
        if (teleRingSpriteRenderer2 != null)
        {
            teleRingSpriteRenderer2.enabled = true;
        }

        allowHurting = true;
        //isStunned = false;

        //if (animator != null)
        //    animator.Play("enemy_spawn", -1);

        //if (psAbsorb != null)
        //    psAbsorb.Play();
    }

    private bool IsGridWalkable(Vector2Int gridPos)
    {
        return
            MapHandler.Instance != null &&
            MapHandler.Instance.MapGrid != null &&
            gridPos.x >= 0 &&
            gridPos.x < MapHandler.Instance.MapGrid.GetWidth() &&
            gridPos.y >= 0 &&
            gridPos.y < MapHandler.Instance.MapGrid.GetHeight() &&
            MapHandler.Instance.MapGrid.GetGridObject(gridPos).Walkable;
    }

    public void Stun(float duration)
    {
        //if (isRespawning)
        //    return;
        StopCoroutine(Death());
        StartCoroutine(Death());
    }

    public bool TryPurge()
    {
        //if (isRespawning)
        //    return false;
        StopCoroutine(Death());
        StartCoroutine(Death());
        return true;
    }

    private IEnumerator Death()
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

        float animDur = 0f;
        if (animator != null)
        {
            animator.Play("qg_scaledown", -1);
            animDur = animator.GetCurrentAnimatorStateInfo(0).length;
        }
        if (teleRingSpriteRenderer1 != null)
        {
            teleRingSpriteRenderer1.enabled = false;
        }
        if (teleRingSpriteRenderer2 != null)
        {
            teleRingSpriteRenderer2.enabled = false;
        }
        yield return new WaitForSeconds(animDur);
        Destroy(gameObject);
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
    }
}
