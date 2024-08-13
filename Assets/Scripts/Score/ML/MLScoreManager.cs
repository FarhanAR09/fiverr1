using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLScoreManager : MonoBehaviour
{
    public static MLScoreManager Instance { get; private set; }

    public float Score { get => score; }
    private float score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        score = 0;
    }

    public void AddScore(float baseScore)
    {
        float multiplier = MemoryTracker.Instance != null && MemoryTracker.Instance.Combo > 0 ?
            Mathf.Max(MemoryTracker.Instance.Combo, Mathf.Pow(2.71828f, 0.6f * MemoryTracker.Instance.Combo) - 1) :
            1;
        score += baseScore * (baseScore > 0 ? multiplier : 1);
        GameEvents.OnMLScoreUpdated.Publish(Score);
    }
}
