using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public static class ScoreCounter
{
    public static int Score { get; private set; }

    /// <summary>
    /// Score updated by amount
    /// </summary>
    public static UnityEvent<int> OnScoreUpdated { get; private set; } = new();
    
    public static void AddScore(int score)
    {
        Score += score;
        OnScoreUpdated.Invoke(Score);
    }

    public static void ResetScore()
    {
        Score = 0;
        OnScoreUpdated.Invoke(Score);
    }
}
