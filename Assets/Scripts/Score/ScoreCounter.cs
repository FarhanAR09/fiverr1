using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public static class ScoreCounter
{
    public static float Score { get; private set; }

    /// <summary>
    /// Score updated by amount
    /// </summary>
    public static UnityEvent<float> OnScoreUpdated { get; private set; } = new();
    
    public static void AddScore(float addedScore)
    {
        float gameSpeed = GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
        Score += addedScore * (gameSpeed >= 0f ? gameSpeed : 1f);
        OnScoreUpdated.Invoke(addedScore);
    }

    public static void ResetScore()
    {
        Score = 0;
        OnScoreUpdated.Invoke(0);
    }
}
