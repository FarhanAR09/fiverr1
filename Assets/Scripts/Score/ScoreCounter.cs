using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreCounter
{
    public static int Score { get; private set; }
    
    public static void AddScore(int score)
    {
        Score += score;
    }
}
