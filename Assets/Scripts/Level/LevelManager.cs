using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles level start and level up (includes controlling time in each level)
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private float levelUpSpeedUp = 0.1f;
    private const string LEVELSPEEDKEY = "LevelTimeSpeedUp";
    private float CurrentSpeed
    {
        get => 1 + level * levelUpSpeedUp;
    }

    private int level = -1;

    private void OnEnable()
    {
        GameEvents.OnAllGatesCollected.Add(LevelUp);
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

        if (!GameSpeedManager.TryModifyGameSpeedModifier(LEVELSPEEDKEY, CurrentSpeed))
            GameSpeedManager.TryAddGameSpeedModifier(LEVELSPEEDKEY, CurrentSpeed);
    }
}
