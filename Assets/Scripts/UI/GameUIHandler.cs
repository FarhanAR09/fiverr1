using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.ShaderGraph.Internal;
#endif
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

    private void UpdateScoreDisplay(float addedScore)
    {
        if (scoreDisplay != null)
            scoreDisplay.SetText("Score:\n" + ScoreCounter.Score.ToString());
    }

    private void UpdateGameSpeedDisplay()
    {
        if (gameSpeedDisplay != null)
            gameSpeedDisplay.SetText("Game Speed:\n" + Time.timeScale.ToString("F2") + " GHz");
    }

    private void UpdateLoseDisplay(bool s)
    {
        float highscore = PlayerPrefs.HasKey("highscore") && PlayerPrefs.GetFloat("highscore") > ScoreCounter.Score ?
            PlayerPrefs.GetFloat("highscore") :
            ScoreCounter.Score;
        if (loseScoreDisplay != null)
            loseScoreDisplay.SetText($"SCORE:\t\t{ScoreCounter.Score}\r\nHIGHSCORE:\t{string.Format("{0:0.##}", highscore)}");
    }
}
