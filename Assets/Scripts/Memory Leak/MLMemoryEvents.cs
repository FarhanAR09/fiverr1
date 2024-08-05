using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static GameEvent<int> OnMLComboUpdated = new();
    public static GameEvent<bool> OnMLComboBroken = new();
    public static GameEvent<int> OnMLMistakesUpdated = new();

    public static GameEvent<bool> OnMLLost = new();
}
