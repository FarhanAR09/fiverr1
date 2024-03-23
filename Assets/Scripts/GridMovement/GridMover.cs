using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridMover : MonoBehaviour, IGridMover
{
    #region Interface Attributes
    public Vector2Int Position { get; private set; }
    public Transform Mover { get; set; }
    public MovementDirection Direction { get; set; } = MovementDirection.Right;
    public float Speed { get; set; }
    #endregion

    private Vector2Int initialPos;

    private bool beenSetUp = false;
    private bool canMove = true;

    #region Interface Methods
    public void ForceMoveTo(Vector2Int position)
    {
        MoveTo(position);
    }
    #endregion

    /// <summary>
    /// Prepares the GridMover before it can be usedS
    /// </summary>
    /// <param name="mover">The transform that is going to be manipulated</param>
    /// <param name="speed"></param>
    /// <param name="initialPos"></param>
    public void SetUp(Transform mover, float speed, Vector2Int initialPos)
    {
        Mover = mover;
        Speed = speed;
        this.initialPos = initialPos;
        beenSetUp = true;
    }

    IEnumerator MoveLoop()
    {
        yield return new WaitUntil(() => beenSetUp);
        MoveTo(initialPos);
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
            yield return new WaitUntil(() => canMove);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool RequestMoveTo(Vector2Int direction)
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
        return false;
    }

    private void MoveTo(Vector2Int gridPosition)
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

                    canMove = true;
                    //Debug.Log("Traverse finished");
                }
                StartCoroutine(TraverseTile());
            }
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(MoveLoop());
    }

    private void OnDestroy()
    {

        StopAllCoroutines();
    }
}
