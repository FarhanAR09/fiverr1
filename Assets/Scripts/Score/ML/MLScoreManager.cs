using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLScoreManager : MonoBehaviour
{
    public static MLScoreManager Instance { get; private set; }

    public float Score { get; private set; }

    private void OnEnable()
    {
        GameEvents.OnMLGameFinished.Add(SetHighscore);
        GameEvents.OnMLLost.Add(SetHighscore);
    }

    private void OnDisable()
    {
        GameEvents.OnMLGameFinished.Remove(SetHighscore);
        GameEvents.OnMLLost.Remove(SetHighscore);
    }

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

        Score = 0;
    }

    public void AddScore(float baseScore)
    {
        float multiplier = MemoryTracker.Instance != null && MemoryTracker.Instance.Combo > 0 ?
            Mathf.Max(MemoryTracker.Instance.Combo, Mathf.Pow(2.71828f, 0.6f * MemoryTracker.Instance.Combo) - 1) :
            1;
        float addedScore = baseScore * (baseScore > 0 ? multiplier : 1);
        Score = Mathf.Max(0f, Score + addedScore);
        GameEvents.OnMLScoreUpdated.Publish(Score);
    }

    private void SetHighscore(bool _)
    {
        print(Score);
        print(PlayerPrefs.GetFloat(GameConstants.MLHIGHSCORE, 0f));
        print("Setting Highscore " + Mathf.Max(
                Score,
                PlayerPrefs.GetFloat(GameConstants.MLHIGHSCORE, 0f)));
        PlayerPrefs.SetFloat(
            GameConstants.MLHIGHSCORE,
            Mathf.Max(
                Score,
                PlayerPrefs.GetFloat(GameConstants.MLHIGHSCORE, 0f)));
        PlayerPrefs.Save();
    }
}
