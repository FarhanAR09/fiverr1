using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static GameEvent<bool> OnMLFlashPowerStarted = new();
    /// <summary>
    /// Normalized cooldown of flash power
    /// </summary>
    public static GameEvent<float> OnMLFlashCDUpdated = new();
    public static GameEvent<bool> OnMLFlashCDStarted = new();
    public static GameEvent<bool> OnMLFlashCDFinished = new();

    /// <summary>
    /// Update freeze state
    /// </summary>
    public static GameEvent<bool> OnMLFreezeStateUpdated = new();
    /// <summary>
    /// Normalized cooldown of freeze power
    /// </summary>
    public static GameEvent<float> OnMLFreezeCDUpdated = new();
    public static GameEvent<bool> OnMLFreezeCDStarted = new();
    public static GameEvent<bool> OnMLFreezeCDFinished = new();
}
