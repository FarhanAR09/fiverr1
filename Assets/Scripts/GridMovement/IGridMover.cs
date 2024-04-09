using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IGridMover
{
    /// <summary>
    /// The transform that is going to be manipulated
    /// </summary>
    public Transform Mover { get; set; }
    public Vector2Int Position { get; }
    public MovementDirection InputDirection { get; set; }
    public float Speed { get; set; }
    public bool BeenSetUp { get; }
    public bool Enabled { get; }

    public UnityEvent OnStartedMoving { get; }
    public UnityEvent OnFinishedMoving { get; }

    public void ForceMoveTo(Vector2Int position);

    public void SetUp(Transform mover, float speed, Vector2Int initialPos, MovementDirection initialDirection);
}
