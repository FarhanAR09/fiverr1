using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static readonly GameEvent<bool> OnPlayerHurt = new();
    public static readonly GameEvent<bool> OnPlayerLose = new();
    public static readonly GameEvent<bool> OnPlayerStopSlowDown = new();
    /// <summary>
    /// Argument is how many life left
    /// </summary>
    public static readonly GameEvent<int> OnLifeUpdated = new();
}