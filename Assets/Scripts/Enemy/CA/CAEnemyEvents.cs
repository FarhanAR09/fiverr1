using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreAttack;

public static partial class GameEvents
{
    public static readonly GameEvent<Enemy> OnCAEnemyDeath = new();
}
