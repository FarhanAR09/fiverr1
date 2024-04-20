using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

public class PlayerInput : MonoBehaviour
{
    //Singleton
    public static GameObject GOInstance { get; private set; }
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

    public UnityEvent OnSpaceDown { get; private set; } = new();
    public UnityEvent OnVDown { get; private set; } = new();

    private bool isMoving = false;

    private void Awake()
    {
        //Singleton
        if (GOInstance != null && GOInstance != this)
        {
            Destroy(gameObject);
            return;
        }
        GOInstance = gameObject;

        //Movement
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, initialPosition, initialDirection);

        Lost = false;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLose.Add(HandleLosing);

        gridMover.OnStartedMoving.AddListener(HandleStartedMoving);
        gridMover.OnFinishedMoving.AddListener(HandleFinishedMoving);
    }

    private void Start()
    {
        StoredDirection = initialDirection;
    }

    private void Update()
    {
        #region Inputs
        #region Movement
        // Translate inputs to direction
        if (!isMoving && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
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
        #endregion
        #endregion
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLose.Remove(HandleLosing);

        gridMover.OnStartedMoving.RemoveListener(HandleStartedMoving);
        gridMover.OnFinishedMoving.RemoveListener(HandleFinishedMoving);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void HandleLosing(bool enabled)
    {
        if (gridMover != null)
        {
            gridMover.Enabled = enabled;
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
}
