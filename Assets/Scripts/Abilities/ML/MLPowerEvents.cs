using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static GameEvent<bool> OnMLFlashPowerStarted = new();
    /// <summary>
    /// Normalized cooldown of flash power
    /// </summary>
    public static GameEvent<float> ONMLFlashCooldownUpdated = new();
    /// <summary>
    /// Update freeze state
    /// </summary>
    public static GameEvent<bool> OnMLFreezeStateUpdated = new();
    /// <summary>
    /// Normalized cooldown of freeze power
    /// </summary>
    public static GameEvent<float> ONMLFreezeCooldownUpdated = new();
}
