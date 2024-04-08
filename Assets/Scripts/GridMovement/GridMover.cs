using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class GridMover : MonoBehaviour, IGridMover
{
    #region Interface Attributes
    public Vector2Int Position { 
        get
        {
            if (_mover != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
                return MapHandler.Instance.MapGrid.GetXY(_mover.transform.position);
            return Vector2Int.zero;
        }
    }
    private Transform _mover;
    public Transform Mover { 
        get
        {
            if (_mover == null)
                Destroy(gameObject);
            return _mover;
        } 
        set => _mover = value; 
    }
    public MovementDirection Direction { get; set; } = MovementDirection.Right;
    public float Speed { get; set; }
    public bool BeenSetUp { get => beenSetUp; private set => beenSetUp = value; }
    public UnityEvent OnStartedMoving { get; } = new();
    public UnityEvent OnFinishedMoving { get; } = new();
    #endregion

    private Vector2Int initialPos;

    private bool beenSetUp = false;
    private bool canMove = true;
    public bool Enabled { get; set; } = true;

    #region Interface Methods
    public void ForceMoveTo(Vector2Int position)
    {
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
        Direction = initialDirection;
        beenSetUp = true;
    }

    IEnumerator MoveLoop()
    {
        yield return new WaitUntil(() => beenSetUp);
        MoveTo(initialPos); //Please move this out
        while (Mover != null && beenSetUp)
        {
            //Debug.Log("Current Direction: " + currentDirection.ToString());
            //Debug.Log("Input");
            switch (Direction)
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
            yield return new WaitUntil(() => canMove && Enabled);
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
                    if (canMove)
                    {
                        //Debug.Log("Current Direction: " + currentDirection.ToString());

                        Vector2Int currentPos = MapHandler.Instance.MapGrid.GetXY(Mover.transform.position);
                        Vector2Int targetPos = currentPos + direction;
                        //Within grid boundary
                        if (targetPos.x >= 0 && targetPos.x < MapHandler.Instance.MapGrid.GetWidth() && targetPos.y >= 0 && targetPos.y < MapHandler.Instance.MapGrid.GetHeight())
                        {
                            //Is target walkable
                            if (MapHandler.Instance.MapGrid.GetGridObject(targetPos).Walkable)
                            {
                                //Debug.Log("Requested");
                                MoveTo(targetPos);

                                return true;
                            }
                        }
                    }
                }
            }
            else Destroy(gameObject);
        }
        return false;
    }

    private void MoveTo(Vector2Int gridPosition)
    {
        if (Enabled)
        {
            if (Mover != null)
            {
                if (MapHandler.Instance != null && canMove)
                {
                    canMove = false;

                    Vector2 worldPosition = MapHandler.Instance.MapGrid.GetWorldPosition(gridPosition.x, gridPosition.y);

                    IEnumerator TraverseTile()
                    {
                        //Debug.Log("Traverse started");

                        OnStartedMoving.Invoke();

                        float duration = 1 / Speed; //Time taken every tile
                        float time = 0;
                        Vector2 initWorldPosition = Mover.transform.position;
                        while (true)
                        {
                            time += Time.fixedDeltaTime;

                            float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                            Mover.transform.position = (Vector3)Vector2.Lerp(initWorldPosition, worldPosition + new Vector2(0.5f * cellSize, 0.5f * cellSize), time / duration) + new Vector3(0, 0, Mover.transform.position.z);

                            if (time >= duration) break;
                            //Debug.Log("Moving");

                            yield return new WaitForFixedUpdate();
                        }

                        OnFinishedMoving.Invoke();
                        canMove = true;
                        //Debug.Log("Traverse finished");
                    }
                    StartCoroutine(TraverseTile());
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
}
