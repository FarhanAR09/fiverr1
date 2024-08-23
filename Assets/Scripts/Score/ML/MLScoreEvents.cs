using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    /// <summary>
    /// Argument: current score
    /// </summary>
    public static GameEvent<float> OnMLScoreUpdated = new();
}
