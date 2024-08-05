using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MLPlayUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreDisplay;

    private void OnEnable()
    {
        GameEvents.OnMLScoreUpdated.Add(UpdateScore);
    }

    private void OnDisable()
    {
        GameEvents.OnMLScoreUpdated.Remove(UpdateScore);
    }

    private void Start()
    {
        UpdateScore(0f);
    }

    private void UpdateScore(float currentScore)
    {
        if (scoreDisplay)
        {
            scoreDisplay.SetText($"SCORE\r\n{currentScore:F0}");
        }
    }
}
