using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    /// <summary>
    /// Publish parameter should be false
    /// </summary>
    public static readonly GameEvent<bool> OnPlayerLose = new();
    public static readonly GameEvent<bool> OnPlayerStopSlowDown = new();
}