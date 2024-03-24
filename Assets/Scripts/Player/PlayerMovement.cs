using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PlayerMovement : MonoBehaviour
{
    //Singleton
    public static GameObject GOInstance { get; private set; }

    //Movement
    [SerializeField]
    [Tooltip("Tile per second")]
    private float speed = 2.5f;
    [SerializeField]
    private Vector2Int initialPosition = Vector2Int.zero;
    [SerializeField]
    private MovementDirection initialDirection = MovementDirection.Right;
    GridMover gridMover;

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
    }

    private void Update()
    {
        #region Inputs
        // Translate inputs to direction
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
}
