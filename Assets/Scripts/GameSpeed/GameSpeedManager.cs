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
        //else if (TryAddGameSpeedModifier(key, value))
        //{
        //    return true;
        //}
        return false;
    }

    public static void RemoveGameSpeedModifier(string key)
    {
        activeModifiers.Remove(key);
        UpdateTimeScale();
    }

    /// <summary>
    /// Returns game speed modifier. If fails, returns -1.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static float TryGetGameSpeedModifier(string key)
    {
        return activeModifiers.ContainsKey(key) ? activeModifiers[key] : -1f;
    }

    private static void UpdateTimeScale()
    {
        Time.timeScale = 1;
        foreach (KeyValuePair<string, float> modifier in activeModifiers)
        {
            //Debug.Log(modifier.Key + ": " + modifier.Value);
            Time.timeScale *= modifier.Value;
        }
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        OnGameSpeedUpdated.Invoke();
    }
}