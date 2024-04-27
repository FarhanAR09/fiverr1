using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.ShaderGraph.Internal;
#endif
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreDisplay, gameSpeedDisplay, loseScoreDisplay;

    private Animator gameSpeedDisplayAnimator;

    private void Start()
    {
        gameSpeedDisplay.TryGetComponent(out gameSpeedDisplayAnimator);
    }

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateScoreDisplay);
        GameSpeedManager.OnGameSpeedUpdated.AddListener(UpdateGameSpeedDisplay);

        GameEvents.OnPlayerLose.Add(UpdateLoseDisplay);

        GameEvents.OnPlayerStopSlowDown.Add(FlashRedReduceGameSpeed);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(UpdateScoreDisplay);
        GameSpeedManager.OnGameSpeedUpdated.RemoveListener(UpdateGameSpeedDisplay);

        GameEvents.OnPlayerLose.Remove(UpdateLoseDisplay);

        GameEvents.OnPlayerStopSlowDown.Remove(FlashRedReduceGameSpeed);
    }

    private void UpdateScoreDisplay(float addedScore)
    {
        if (scoreDisplay != null)
            scoreDisplay.SetText("Score:\n" + ScoreCounter.Score.ToString("F0"));
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
            loseScoreDisplay.SetText($"SCORE:\t\t{ScoreCounter.Score}\r\nHIGHSCORE:\t{highscore:F0}");
    }

    private void FlashRedReduceGameSpeed(bool _)
    {
        if (gameSpeedDisplayAnimator != null)
            gameSpeedDisplayAnimator.Play("gamespeed_flicker");
    }
}
