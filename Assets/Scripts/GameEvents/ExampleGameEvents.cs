using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for making custom GameEvent. Can be made from different files with different filename.
/// Usage: Make new file (whatever filename). Declare class like below (public static partial class GameEvents). Fill class like below (can contain more than 1 event).
/// </summary>
public static partial class GameEvents
{
    public static readonly GameEvent<bool> OnExampleEventTriggered1 = new();
    public static readonly GameEvent<float> OnExampleEventTriggered2 = new();
}