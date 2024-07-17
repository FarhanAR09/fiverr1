using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class GameEvents
{
    public static GameEvent<bool> OnSwitchPurge = new();

    public static GameEvent<bool> OnSwitchSpeedUp = new();

    public static GameEvent<bool> OnSwitchSlowDown = new();

    public static GameEvent<bool> OnSwitchRequireCharge = new();

    public static GameEvent<bool> OnSwitchCooldownCharge = new();

    public static GameEvent<bool> OnSwitchPowersCooldown = new();

    public static GameEvent<int> OnUpdateCacheOverflow = new();

    public static GameEvent<bool> OnSwitchSequentialGates = new();
    
    public static GameEvent<bool> OnSwitchIdleMechanics = new();

    public static GameEvent<bool> OnSwitchSpawnBitsEaters = new();
    public static GameEvent<bool> OnSwitchSpawnTrojanHorses = new();
    public static GameEvent<bool> OnSwitchSpawnQuantumGhosts = new();
}