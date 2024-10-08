using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles level start and level up (includes controlling time in each level)
/// </summary>
public class LevelManager : MonoBehaviour
{
    private static int level = -1;
    private bool speedUpEnabled = true;

    private void OnEnable()
    {
        GameEvents.OnAllGatesCollected.Add(LevelUp);

        GameEvents.OnSwitchSpeedUp.Add(HandleFeatureSwitch);
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

        GameEvents.OnSwitchSpeedUp.Remove(HandleFeatureSwitch);
    }

    private void OnDestroy()
    {
        GameSpeedManager.RemoveGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
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

        if (speedUpEnabled)
        {
            if (!GameSpeedManager.TryModifyGameSpeedModifier(GameConstants.LEVELSPEEDKEY, GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY) + GameConstants.LEVELUPSPEEDUP))
                GameSpeedManager.TryAddGameSpeedModifier(GameConstants.LEVELSPEEDKEY, 1f + level * GameConstants.LEVELUPSPEEDUP);
        }
    }

    private void HandleFeatureSwitch(bool state)
    {
        speedUpEnabled = state;
        if (state)
        {
            if (!GameSpeedManager.TryModifyGameSpeedModifier(GameConstants.LEVELSPEEDKEY, GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY) + GameConstants.LEVELUPSPEEDUP))
                GameSpeedManager.TryAddGameSpeedModifier(GameConstants.LEVELSPEEDKEY, 1f + level * GameConstants.LEVELUPSPEEDUP);
        }
        else
        {
            GameSpeedManager.RemoveGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
        }
    }
}
