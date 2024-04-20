using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    public static GameEvent<bool> OnPurgeStarted = new();
    public static GameEvent<bool> OnPurgeFinished = new();
}
