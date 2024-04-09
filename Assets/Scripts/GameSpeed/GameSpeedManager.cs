using System.Collections;
using System.Collections.Generic;
using System.Xml;
#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;
using UnityEngine.Events;

public static class GameSpeedManager
{
    private static Dictionary<string, float> activeModifiers = new();
    public static UnityEvent OnGameSpeedUpdated { get; private set; } = new();

    public static bool TryAddGameSpeedModifier(string key, float value)
    {
        bool success = activeModifiers.TryAdd(key, value);
        if (success)
            UpdateTimeScale();
        return success;
    }

    public static bool TryModifyGameSpeedModifier(string key, float value)
    {
        if (activeModifiers.ContainsKey(key))
        {
            activeModifiers[key] = value;
            UpdateTimeScale();
            return true;
        }
        return false;
    }

    public static void RemoveGameSpeedModifier(string key)
    {
        activeModifiers.Remove(key);
        UpdateTimeScale();
    }

    private static void UpdateTimeScale()
    {
        Time.timeScale = 1;
        foreach (KeyValuePair<string, float> modifier in activeModifiers)
        {
            Time.timeScale *= modifier.Value;
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        OnGameSpeedUpdated.Invoke();
    }
}