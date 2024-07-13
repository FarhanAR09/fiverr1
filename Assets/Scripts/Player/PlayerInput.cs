using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

public class PlayerInput : MonoBehaviour, IEnemyHurtable
{
    //Singleton
    public static GameObject GOInstance { get => Instance.gameObject; }
    public static PlayerInput Instance { get; private set; }
    public bool Lost { get; private set; } = false;

    //Movement
    [SerializeField]
    [Tooltip("Tile per second")]
    private float speed = 2.5f;
    [SerializeField]
    private Vector2Int initialPosition = Vector2Int.zero;
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    private GridMover gridMover;
    public MovementDirection StoredDirection { get; private set; }

    //Input Events
    public UnityEvent OnSpaceDown { get; private set; } = new();
    public UnityEvent OnVDown { get; private set; } = new();
    public UnityEvent OnBoostDown { get; private set; } = new();
    public UnityEvent OnBoostUp { get; private set; } = new();
    public UnityEvent OnHitWall { get; private set; } = new();

    private bool isMoving = false;

    //Reduce GameSpeed by Stopping
    private Vector2 lastFramePos = Vector2.zero;
    private float playerStoppedTimer;
    private readonly float reduceGameSpeedStopDuration = 1f;

    //Hit Wall
    bool wallHit = false;

    //Boost Override
    private bool isBoosting = false;
    private PlayerPowerUpManager powerManager;
    private readonly float boostSpeedMultiplier = 2f;
    private float speedBeforeBoost;

    //Life
    public static int Life { get; private set; } = 1;
    /// <summary>
    /// Can only collect life once every level
    /// </summary>
    private bool lifeCollectedInThisLevel = false;

    //Invincibility
    private bool invincible = false;

    //Animations
    private Animator animator;

    //Debugging
    private bool canSlowDown = true;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Movement
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, initialPosition, initialDirection);

        Lost = false;

        TryGetComponent(out powerManager);

        TryGetComponent(out animator);
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLose.Add(HandleLosing);

        gridMover.OnStartedMoving.AddListener(HandleStartedMoving);
        gridMover.OnFinishedMoving.AddListener(HandleFinishedMoving);
        
        gridMover.OnStartedMoving.AddListener(HitWallStateFalse);
        gridMover.OnDeniedMoving.AddListener(InvokeHitWall);

        if (powerManager != null)
        {
            powerManager.OnBoostStart.AddListener(StartBoostMovementOverride);
            powerManager.OnBoostEnd.AddListener(EndBoostMovementOverride);
        }

        GameEvents.OnSwitchSlowDown.Add(SlowDownState);

        //Test
        //GameEvents.OnLifeUpdated.Add(DebugDisplayLife);

        GameEvents.OnCacheOverflowed.Add(TryCollectLife);
        GameEvents.OnLevelUp.Add(AllowLifeCollection);
    }

    private void Start()
    {
        StoredDirection = initialDirection;

        //Life
        SetLife(1);
    }

    private void Update()
    {
        #region Inputs
        #region Movement
        // Translate inputs to direction
        if (!isMoving && !isBoosting && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            Vector2Int gridPos = MapHandler.Instance.MapGrid.GetXY(transform.position);

            if (MapHandler.Instance.CheckBoundary(gridPos))
            {
                Vector2Int checkedPos;
                if (Input.GetButton("Up"))
                {
                    checkedPos = gridPos + Vector2Int.up;
                    if (MapHandler.Instance.CheckBoundary(checkedPos) && MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable)
                    {
                        gridMover.InputDirection = MovementDirection.Up;
                    }
                    //Debug.Log("Up: " + MapHandler.Instance.CheckBoundary(checkedPos) + " " + MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable);
                }
                else if (Input.GetButton("Down"))
                {
                    checkedPos = gridPos + Vector2Int.down;
                    if (MapHandler.Instance.CheckBoundary(checkedPos) && MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable)
                    {
                        gridMover.InputDirection = MovementDirection.Down;
                    }
                    //Debug.Log("Down: " + MapHandler.Instance.CheckBoundary(checkedPos) + " " + MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable);
                }
                else if (Input.GetButton("Left"))
                {
                    checkedPos = gridPos + Vector2Int.left;
                    if (MapHandler.Instance.CheckBoundary(checkedPos) && MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable)
                    {
                        gridMover.InputDirection = MovementDirection.Left;
                    }
                    //Debug.Log("Left: " + MapHandler.Instance.CheckBoundary(checkedPos) + " " + MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable);
                }
                else if (Input.GetButton("Right"))
                {
                    checkedPos = gridPos + Vector2Int.right;
                    if (MapHandler.Instance.CheckBoundary(checkedPos) && MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable)
                    {
                        gridMover.InputDirection = MovementDirection.Right;
                    }
                    //Debug.Log("Right: " + MapHandler.Instance.CheckBoundary(checkedPos) + " " + MapHandler.Instance.MapGrid.GetGridObject(checkedPos).Walkable);
                }
                StoredDirection = gridMover.InputDirection;
                //else if (DirectionUtils.MovementDirectionToVector2Int(StoredDirection) == -DirectionUtils.MovementDirectionToVector2Int(gridMover.CurrentDirection))
                //{
                //   gridMover.ForceMoveTo(gridPos + DirectionUtils.MovementDirectionToVector2Int(StoredDirection));
                //}
            }
        }
        #endregion

        #region Power Up
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpaceDown.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            OnVDown.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnBoostDown.Invoke();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            OnBoostUp.Invoke();
        }
        #endregion
        #endregion
    }

    private void FixedUpdate()
    {
        #region Player Stop Slow Down
        if ((lastFramePos - (Vector2)transform.position).sqrMagnitude < 0.01f)
        {
            if (playerStoppedTimer < reduceGameSpeedStopDuration)
            {
                playerStoppedTimer += Time.fixedDeltaTime;
            }
            else
            {
                playerStoppedTimer = 0;
                if (canSlowDown)
                {
                    float currentSpeed = GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
                    float reducedGameSpeed = Mathf.Max(currentSpeed - GameConstants.PLAYERSTOPSLOWDOWN, 1f);
                    GameSpeedManager.TryModifyGameSpeedModifier(GameConstants.LEVELSPEEDKEY, reducedGameSpeed);
                    if (currentSpeed - reducedGameSpeed > 0f)
                    {
                        GameEvents.OnPlayerStopSlowDown.Publish(true);
                    }
                }
            }
        }
        else
        {
            playerStoppedTimer = 0;
        }
        #endregion

        //Please keep at the end of FixedUpdate
        lastFramePos = transform.position;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLose.Remove(HandleLosing);

        gridMover.OnStartedMoving.RemoveListener(HandleStartedMoving);
        gridMover.OnFinishedMoving.RemoveListener(HandleFinishedMoving);
        
        gridMover.OnStartedMoving.RemoveListener(HitWallStateFalse);
        gridMover.OnDeniedMoving.RemoveListener(InvokeHitWall);

        if (powerManager != null)
        {
            powerManager.OnBoostStart.RemoveListener(StartBoostMovementOverride);
            powerManager.OnBoostEnd.RemoveListener(EndBoostMovementOverride);
        }

        GameEvents.OnSwitchSlowDown.Remove(SlowDownState);

        //Test
        //GameEvents.OnLifeUpdated.Remove(DebugDisplayLife);

        GameEvents.OnCacheOverflowed.Remove(TryCollectLife);
        GameEvents.OnLevelUp.Remove(AllowLifeCollection);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void HandleLosing(bool _)
    {
        if (gridMover != null)
        {
            gridMover.SetActiveState(false);
        }
        Lost = true;
    }

    private void HandleStartedMoving()
    {
        isMoving = true;
    }

    private void HandleFinishedMoving()
    {
        isMoving = false;
    }

    private void HitWallStateFalse()
    {
        wallHit = false;
    }

    private void InvokeHitWall()
    {
        if (!wallHit)
        {
            wallHit = true;
            OnHitWall.Invoke();
        }
    }

    private void StartBoostMovementOverride()
    {
        if (powerManager != null)
        {
            isBoosting = true;
            speedBeforeBoost = gridMover.Speed;
            gridMover.Speed *= boostSpeedMultiplier;
        }
    }

    private void EndBoostMovementOverride()
    {
        if (isBoosting)
        {
            isBoosting = false;
            gridMover.Speed = speedBeforeBoost;
        }
    }

    private void SlowDownState(bool state)
    {
        canSlowDown = state;
    }

    public void SetLife(int amount)
    {
        Life = amount;
        GameEvents.OnLifeUpdated.Publish(Life);
    }

    //private void DebugDisplayLife(int remainingLives)
    //{
    //    Debug.Log("Remaining Lives: " + remainingLives);
    //}

    public bool TryHurt()
    {
        if (invincible || Lost)
        {
            return false;
        }

        SetLife(Life - 1);
        GameEvents.OnPlayerHurt.Publish(true);
        if (Life <= 0)
        {
            Lost = true;
            GameEvents.OnPlayerLose.Publish(true);
        }
        else
        {
            IEnumerator Invincibility()
            {
                invincible = true;
                if (animator != null)
                {
                    animator.Play("player_invincible", -1);
                }

                yield return new WaitForSeconds(3f);

                invincible = false;
                if (animator != null)
                {
                    animator.Play("player_noAnim", 1);
                }
            }
            StopCoroutine(Invincibility());
            StartCoroutine(Invincibility());
        }
        return true;
    }

    private void TryCollectLife(bool _)
    {
        if (!lifeCollectedInThisLevel && Life < 5)
        {
            lifeCollectedInThisLevel = true;
            SetLife(Life + 1);
        }
    }

    private void AllowLifeCollection(bool _)
    {
        lifeCollectedInThisLevel = false;
    }
}
