using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    public static GameEvent<bool> OnCacheOverflowed = new();
    public static GameEvent<ScorePellet> OnBitInitialized = new();
    public static GameEvent<ScorePellet> OnBitCorrupted = new();
    /// <summary>
    /// Credit is updated to int amount
    /// </summary>
    public static GameEvent<int> OnCreditUpdated = new();
}
