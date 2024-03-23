using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridMover
{
    /// <summary>
    /// The transform that is going to be manipulated
    /// </summary>
    public Transform Mover { get; set; }
    public Vector2Int Position { get; }
    public MovementDirection Direction { get; set; }
    public float Speed { get; set; }

    public void ForceMoveTo(Vector2Int position);

    public void SetUp(Transform mover, float speed, Vector2Int initialPos);
}
