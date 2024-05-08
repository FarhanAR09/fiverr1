using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameEvents
{
    public static readonly GameEvent<float> OnEMPCooldownUpdated = new();
    public static readonly GameEvent<bool> OnEMPCooldownStarted = new();
    public static readonly GameEvent<bool> OnEMPCooldownFinished = new();

    public static readonly GameEvent<float> OnBulletTimeCooldownUpdated = new();
    public static readonly GameEvent<bool> OnBulletTimeCooldownStarted = new();
    public static readonly GameEvent<bool> OnBulletTimeCooldownFinished = new();

    public static readonly GameEvent<float> OnBoostCooldownUpdated = new();
    public static readonly GameEvent<bool> OnBoostCooldownStarted = new();
    public static readonly GameEvent<bool> OnBoostCooldownFinished = new();
}