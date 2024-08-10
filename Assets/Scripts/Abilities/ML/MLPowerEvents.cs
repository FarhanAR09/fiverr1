using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static GameEvent<bool> OnMLFlashPowerStarted = new();
    /// <summary>
    /// Update freeze state
    /// </summary>
    public static GameEvent<bool> OnMLFreezePowerUpdated = new();
}
