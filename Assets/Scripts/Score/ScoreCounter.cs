using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

public static class ScoreCounter
{
    public static float Score { get; private set; }
    public static float CorruptedScore { get; private set; }
    public static float TotalScore
    {
        get => Score - CorruptedScore;
    }

    /// <summary>
    /// Score updated by amount
    /// </summary>
    public static UnityEvent<float> OnScoreUpdated { get; private set; } = new();
    
    public static void AddScore(float addedScore)
    {
        float gameSpeed = GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
        float finalAddedScore = addedScore * (gameSpeed >= 0f ? gameSpeed : 1f);
        Score += finalAddedScore;
        OnScoreUpdated.Invoke(finalAddedScore);
    }

    public static void AddCorruptedScore(float addedScore)
    {
        float gameSpeed = GameSpeedManager.TryGetGameSpeedModifier(GameConstants.LEVELSPEEDKEY);
        float finalAddedScore = addedScore * (gameSpeed >= 0f ? gameSpeed : 1f);
        CorruptedScore += finalAddedScore;
        OnScoreUpdated.Invoke(-finalAddedScore);
    }

    public static void ResetScore()
    {
        Score = 0;
        CorruptedScore = 0;
        OnScoreUpdated.Invoke(0);
    }
}
