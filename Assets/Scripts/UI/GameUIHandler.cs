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
    private TextMeshProUGUI scoreDisplay, gameSpeedDisplay, loseScoreDisplay, loseCreditsDisplay, leaderboardScoreDisplay;

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
        GameEvents.OnPlayerLose.Add(UpdateLeaderboardScoreDisplay);

        GameEvents.OnPlayerStopSlowDown.Add(FlashRedReduceGameSpeed);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(UpdateScoreDisplay);
        GameSpeedManager.OnGameSpeedUpdated.RemoveListener(UpdateGameSpeedDisplay);

        GameEvents.OnPlayerLose.Remove(UpdateLoseDisplay);
        GameEvents.OnPlayerLose.Remove(UpdateLeaderboardScoreDisplay);

        GameEvents.OnPlayerStopSlowDown.Remove(FlashRedReduceGameSpeed);
    }

    private void UpdateScoreDisplay(float addedScore)
    {
        if (scoreDisplay != null)
            scoreDisplay.SetText("Score:\n" + ScoreCounter.TotalScore.ToString("F0"));
    }

    private void UpdateGameSpeedDisplay()
    {
        if (gameSpeedDisplay != null)
            gameSpeedDisplay.SetText("Game Speed:\n" + Time.timeScale.ToString("F2") + " GHz");
    }

    private void UpdateLoseDisplay(bool _)
    {
        float highscore = PlayerPrefs.HasKey("highscore") && PlayerPrefs.GetFloat("highscore") > ScoreCounter.TotalScore ?
            PlayerPrefs.GetFloat("highscore") :
            ScoreCounter.TotalScore;
        if (loseScoreDisplay != null)
            loseScoreDisplay.SetText(
                $"SCORE:\t\t\t{ScoreCounter.Score}\r\n" +
                $"CORRUPTED SCORE:\t{ScoreCounter.CorruptedScore}\r\n" +
                $"TOTAL SCORE:\t\t{ScoreCounter.TotalScore}\r\n" +
                $"HIGHSCORE:\t\t{highscore:F0}\r\n");
        if (loseCreditsDisplay != null)
            loseCreditsDisplay.SetText($"+{ScoreCounter.TotalScore} CREDITS EARNED");
    }

    private void FlashRedReduceGameSpeed(bool _)
    {
        if (gameSpeedDisplayAnimator != null)
            gameSpeedDisplayAnimator.Play("gamespeed_flicker");
    }

    private void UpdateLeaderboardScoreDisplay(bool _)
    {
        if (leaderboardScoreDisplay != null)
        {
            leaderboardScoreDisplay.SetText($"{ScoreCounter.TotalScore:F0}");
        }
    }
}
