using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.PackageManager;
using UnityEngine;

public static class GameSpeedManager
{
    private static Dictionary<string, float> activeModifiers = new();

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
    }
}