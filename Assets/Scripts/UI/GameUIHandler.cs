using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreDisplay, gameSpeedDisplay, loseScoreDisplay;

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateScoreDisplay);
        GameSpeedManager.OnGameSpeedUpdated.AddListener(UpdateGameSpeedDisplay);

        GameEvents.OnPlayerLose.Add(UpdateLoseDisplay);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(UpdateScoreDisplay);
        GameSpeedManager.OnGameSpeedUpdated.RemoveListener(UpdateGameSpeedDisplay);

        GameEvents.OnPlayerLose.Remove(UpdateLoseDisplay);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (scoreDisplay != null)
            scoreDisplay.SetText("Score:\n" + score.ToString());
    }

    private void UpdateGameSpeedDisplay()
    {
        if (gameSpeedDisplay != null)
            gameSpeedDisplay.SetText("Game Speed:\n" + Time.timeScale.ToString("F6"));
    }

    private void UpdateLoseDisplay(bool s)
    {
        int highscore = PlayerPrefs.HasKey("highscore") && PlayerPrefs.GetInt("highscore") > ScoreCounter.Score ?
            PlayerPrefs.GetInt("highscore") :
            ScoreCounter.Score;
        if (loseScoreDisplay != null)
            loseScoreDisplay.SetText($"SCORE:\t\t{ScoreCounter.Score}\r\nHIGHSCORE:\t{highscore}");
    }
}
