using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles level start and level up (includes controlling time in each level)
/// </summary>
public class LevelManager : MonoBehaviour
{
    private static readonly float levelUpSpeedUp = 0.1f;
    private const string LEVELSPEEDKEY = "LevelTimeSpeedUp";
    public static float CurrentLevelSpeed
    {
        get => 1f + level * levelUpSpeedUp;
    }

    private static int level = -1;

    private void OnEnable()
    {
        GameEvents.OnAllGatesCollected.Add(LevelUp);
    }

    private void Awake()
    {
        level = -1;
    }

    private void Start()
    {
        InstantLevelUp();
    }

    private void OnDisable()
    {
        GameEvents.OnAllGatesCollected.Remove(LevelUp);
    }

    private void OnDestroy()
    {
        GameSpeedManager.RemoveGameSpeedModifier(LEVELSPEEDKEY);
    }

    private void LevelUp(bool _)
    {
        IEnumerator DelayLevelUp()
        {
            yield return new WaitForSecondsRealtime(1f);

            InstantLevelUp();
        }
        StopCoroutine(DelayLevelUp());
        StartCoroutine(DelayLevelUp());
    }

    private void InstantLevelUp()
    {
        level++;

        GameEvents.OnLevelUp.Publish(true);

        if (!GameSpeedManager.TryModifyGameSpeedModifier(LEVELSPEEDKEY, CurrentLevelSpeed))
            GameSpeedManager.TryAddGameSpeedModifier(LEVELSPEEDKEY, CurrentLevelSpeed);
    }
}
