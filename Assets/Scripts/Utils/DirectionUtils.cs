using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DirectionUtils
{
    public static Vector2Int MovementDirectionToVector2Int(MovementDirection movementDirection)
    {
        switch (movementDirection)
        {
            case MovementDirection.Up:
                return Vector2Int.up;
            case MovementDirection.Down:
                return Vector2Int.down;
            case MovementDirection.Left:
                return Vector2Int.left;
            case MovementDirection.Right:
                return Vector2Int.right;
            default:
                return Vector2Int.zero;
        }
    }

    public static MovementDirection Vector2IntToMovementDirection(Vector2Int vector)
    {
        if (vector == Vector2Int.up)
        {
            return MovementDirection.Up;
        }
        else if (vector == Vector2Int.down)
        {
            return MovementDirection.Down;
        }
        else if (vector == Vector2Int.left)
        {
            return MovementDirection.Left;
        }
        else if (vector == Vector2Int.right)
        {
            return MovementDirection.Right;
        }
        else
        {
            return MovementDirection.Up;
        }
    }
}