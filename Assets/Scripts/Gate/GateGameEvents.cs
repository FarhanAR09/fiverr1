using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    public static GameEvent<bool> OnAllGatesCollected = new();
    public static GameEvent<bool> OnGatesWrongSequence = new();
    public static GameEvent<int> OnGatesSequenceUpdate = new();
}
