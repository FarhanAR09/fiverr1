using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerMovement : MonoBehaviour
{
    private MovementDirection currentDirection = MovementDirection.Right;
    private bool canMove = true;

    [SerializeField]
    [Tooltip("Tile per second")]
    private float speed = 2.5f;

    [SerializeField]
    private bool smooth = false;

    GridMover gridMover;

    private void Awake()
    {
        gridMover = new GameObject("Player Grid Mover", typeof(GridMover)).GetComponent<GridMover>();
        gridMover.transform.parent = transform;
        gridMover.SetUp(transform, speed, new Vector2Int(0, 0));
    }

    private void Start()
    {
        //MoveTo(new(0, 0));
        //StartCoroutine(MoveLoop());
    }

    private void Update()
    {
        #region Inputs
        if (Input.GetButton("Up"))
        {
            gridMover.Direction = MovementDirection.Up;
        }
        else if (Input.GetButton("Down"))
        {
            gridMover.Direction = MovementDirection.Down;
        }
        else if (Input.GetButton("Left"))
        {
            gridMover.Direction = MovementDirection.Left;
        }
        else if (Input.GetButton("Right"))
        {
            gridMover.Direction = MovementDirection.Right;
        }
        #endregion
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    
    IEnumerator MoveLoop()
    {
        while (true)
        {
            //Debug.Log("Current Direction: " + currentDirection.ToString());
            //Debug.Log("Input");
            switch (currentDirection)
            {
                case MovementDirection.Up:
                    RequestMoveTo(new(0, 1));
                    break;
                case MovementDirection.Down:
                    RequestMoveTo(new(0, -1));
                    break;
                case MovementDirection.Left:
                    RequestMoveTo(new(-1, 0));
                    break;
                case MovementDirection.Right:
                    RequestMoveTo(new(1, 0));
                    break;
            }

            //Debug.Log("Waiting Move Loop...");
            yield return new WaitUntil(() => canMove);
            yield return new WaitForFixedUpdate();
        }
    }

    private bool RequestMoveTo(Vector2Int direction)
    {
        if (MapHandler.Instance != null)
        {
            if (canMove)
            {
                //Debug.Log("Current Direction: " + currentDirection.ToString());

                Vector2Int currentPos = MapHandler.Instance.MapGrid.GetXY(transform.position);
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
        return false;
    }

    private void MoveTo(Vector2Int gridPosition)
    {
        if (MapHandler.Instance != null && canMove)
        {
            canMove = false;

            Vector2 worldPosition = MapHandler.Instance.MapGrid.GetWorldPosition(gridPosition.x, gridPosition.y);

            IEnumerator TraverseTile()
            {
                //Debug.Log("Traverse started");
                
                if (smooth)
                {
                    float duration = 1 / speed; //Time taken every tile
                    float time = 0;
                    Vector2 initWorldPosition = transform.position;
                    while (true)
                    {
                        time += Time.fixedDeltaTime;

                        float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                        transform.position = (Vector3)Vector2.Lerp(initWorldPosition, worldPosition + new Vector2(0.5f * cellSize, 0.5f * cellSize), time / duration) + new Vector3(0, 0, transform.position.z);

                        if (time >= duration) break;
                        //Debug.Log("Moving");

                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                {
                    float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                    transform.position = worldPosition + new Vector2(0.5f * cellSize, 0.5f * cellSize);
                    yield return new WaitForSeconds(1 / speed);
                }

                canMove = true;
                //Debug.Log("Traverse finished");
            }
            StartCoroutine(TraverseTile());
        }
    }
}
