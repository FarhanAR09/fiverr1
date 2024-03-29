using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class ScoreCounter
{
    public static int Score { get; private set; }

    public static UnityEvent<int> OnScoreUpdated { get; private set; } = new();
    
    public static void AddScore(int score)
    {
        Score += score;
        OnScoreUpdated.Invoke(Score);
    }
}
