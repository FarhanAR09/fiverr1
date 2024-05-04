using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    public static GameEvent<bool> OnCacheOverflowed = new();
    public static GameEvent<ScorePellet> OnBitCorrupted = new();
}
