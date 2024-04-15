using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResponsiveGridMover : MonoBehaviour, IGridMover
{
    #region Interface Attributes
    public Vector2Int Position
    {
        get
        {
            if (_mover != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
                return MapHandler.Instance.MapGrid.GetXY(_mover.transform.position);
            return Vector2Int.zero;
        }
    }
    private Transform _mover;
    public Transform Mover
    {
        get
        {
            if (_mover == null)
                Destroy(gameObject);
            return _mover;
        }
        set => _mover = value;
    }
    public MovementDirection InputDirection { get; set; } = MovementDirection.Right;
    public MovementDirection CurrentDirection { get; private set; } = MovementDirection.Right;
    public float Speed { get; set; }
    public bool BeenSetUp { get => beenSetUp; private set => beenSetUp = value; }
    public UnityEvent OnStartedMoving { get; } = new();
    public UnityEvent OnFinishedMoving { get; } = new();
    #endregion

    private Vector2Int initialPos;

    private bool beenSetUp = false;
    //private bool finishedMoving = true;
    public bool Enabled { get; set; } = true;

    private Rigidbody2D moverRb;

    private Coroutine tileTraversal;

    #region Interface Methods
    public void ForceMoveTo(Vector2Int position)
    {
        CancelTileTraversal();
        MoveTo(position);
    }
    #endregion

    /// <summary>
    ///Prepares the GridMover before it can be used
    /// </summary>
    /// <param name="mover">The transform that is going to be manipulated</param>
    /// <param name="speed">Tiles per second</param>
    /// <param name="initialPos">Where will the mover starts moving?</param>
    public void SetUp(Transform mover, float speed, Vector2Int initialPos, MovementDirection initialDirection)
    {
        Mover = mover;
        Speed = speed;
        this.initialPos = initialPos;
        InputDirection = initialDirection;
        beenSetUp = true;

        if (Mover.TryGetComponent(out Rigidbody2D _rb))
        {
            moverRb = _rb;
        }
    }

    IEnumerator MoveLoop()
    {
        yield return new WaitUntil(() => beenSetUp);
        MoveTo(initialPos); //Please move this out
        while (Mover != null && beenSetUp)
        {
            //Debug.Log("Current Direction: " + currentDirection.ToString());
            //Debug.Log("Input");

            switch (InputDirection)
            {
                case MovementDirection.Up:
                    RequestMoveTo(new Vector2Int(0, 1));
                    break;
                case MovementDirection.Down:
                    RequestMoveTo(new Vector2Int(0, -1));
                    break;
                case MovementDirection.Left:
                    RequestMoveTo(new Vector2Int(-1, 0));
                    break;
                case MovementDirection.Right:
                    RequestMoveTo(new Vector2Int(1, 0));
                    break;
            }

            //Debug.Log("Waiting Move Loop...");
            yield return new WaitUntil(() => Enabled);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool RequestMoveTo(Vector2Int direction)
    {
        if (Enabled)
        {
            if (Mover != null)
            {
                if (MapHandler.Instance != null)
                {
                    Vector2Int currentGridPos = MapHandler.Instance.MapGrid.GetXY(Mover.transform.position);
                    Vector2Int targetPos = currentGridPos + direction;

                    //Within grid boundary
                    if (targetPos.x >= 0 && targetPos.x < MapHandler.Instance.MapGrid.GetWidth() && targetPos.y >= 0 && targetPos.y < MapHandler.Instance.MapGrid.GetHeight())
                    {
                        //Is target walkable
                        if (MapHandler.Instance.MapGrid.GetGridObject(targetPos).Walkable)
                        {
                            //Debug.Log("Requested");
                            CurrentDirection = DirectionUtils.Vector2IntToMovementDirection(direction);
                            
                            float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                            Vector2 gridWorldPos = MapHandler.Instance.MapGrid.GetWorldPosition(currentGridPos.x, currentGridPos.y) + new Vector3(cellSize / 2, cellSize / 2);
                            //Align Vertical Position
                            if (CurrentDirection == MovementDirection.Left || CurrentDirection == MovementDirection.Right)
                            {
                                Mover.position = Vector3.Lerp(Mover.position, new Vector3(Mover.position.x, gridWorldPos.y, Mover.position.z), 0.3f);
                            }
                            //Align Horizontal Position
                            else if (CurrentDirection == MovementDirection.Up || CurrentDirection == MovementDirection.Down)
                            {
                                Mover.position = Vector3.Lerp(Mover.position, new Vector3(gridWorldPos.x, Mover.position.y, Mover.position.z), 0.3f);
                            }
                            //if (((Vector2)Mover.position - gridWorldPos).sqrMagnitude < 0.125f)
                            //{
                            //
                            //}

                            if (moverRb != null)
                            {
                                moverRb.velocity = Speed * (Vector2)direction;
                            }

                            return true;
                        }
                        else if (moverRb != null)
                            moverRb.velocity = Vector2.zero;
                    }
                    else if (moverRb != null)
                        moverRb.velocity = Vector2.zero;
                }
            }
            else Destroy(gameObject);
        }
        else if (moverRb != null)
            moverRb.velocity = Vector2.zero;
        return false;
    }

    private void MoveTo(Vector2Int gridPosition)
    {
        if (Enabled)
        {
            if (Mover != null)
            {
                if (MapHandler.Instance != null)
                {
                    Vector2 worldPosition = MapHandler.Instance.MapGrid.GetWorldPosition(gridPosition.x, gridPosition.y);

                    
                }
            }
            else Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(MoveLoop());
    }

    private void OnDisable()
    {
        StopCoroutine(MoveLoop());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void CancelTileTraversal()
    {
        StopCoroutine(tileTraversal);
        OnFinishedMoving.Invoke();
        //finishedMoving = true;
    }
}
