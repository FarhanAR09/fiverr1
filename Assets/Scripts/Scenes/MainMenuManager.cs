using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highscoreDisplay;
    [SerializeField]
    private GameObject panelMainMenu, panelLeaderboard, panelHowToPlay, panelSettings;
    [SerializeField]
    private TextMeshProUGUI leaderNamesDisplay, leaderScoresDisplay;

    private void Start()
    {
        float highscore = PlayerPrefs.HasKey("highscore") ?
            PlayerPrefs.GetFloat("highscore") :
            0;
        if (highscoreDisplay != null)
            highscoreDisplay.SetText($"HIGHSCORE: {string.Format("{0:0.##}", highscore)}");

        if (MusicController.Instance != null)
        {
            MusicController.Instance.Stop();
            MusicController.Instance.Play();
        }

        if (panelMainMenu != null)
        {
            panelMainMenu.SetActive(true);
        }
        if (panelLeaderboard != null)
        {
            panelLeaderboard.SetActive(false);
        }
        if (panelHowToPlay != null)
        {
            panelHowToPlay.SetActive(false);
        }
        if (panelSettings != null)
        {
            panelSettings.SetActive(false);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CloseLeaderboard()
    {
        if (panelMainMenu != null)
            panelMainMenu.SetActive(true);
        if (panelLeaderboard != null)
            panelLeaderboard.SetActive(false);
    }

    public void OpenLeaderboard()
    {
        if (panelMainMenu != null)
            panelMainMenu.SetActive(false);
        if (panelLeaderboard != null)
            panelLeaderboard.SetActive(true);
        List<KeyValuePair<string, float>> namesAndScores = LeaderboardDataManager.GetListSorted();
        if (leaderNamesDisplay)
        {
            string leaderNames = "";
            foreach(KeyValuePair<string, float> entry in namesAndScores)
            {
                leaderNames += entry.Key + "\n";
            }
            leaderNamesDisplay.SetText(leaderNames);
        }
        if (leaderScoresDisplay)
        {
            string leaderScores = "";
            foreach (KeyValuePair<string, float> entry in namesAndScores)
            {
                leaderScores += entry.Value + "\n";
            }
            leaderScoresDisplay.SetText(leaderScores);
        }
    }

    public void OpenHowToPlay()
    {
        if (panelMainMenu != null)
        {
            panelMainMenu.SetActive(false);
        }
        if (panelHowToPlay != null)
        {
            panelHowToPlay.SetActive(true);
        }
    }

    public void CloseHowToPlay()
    {
        if (panelMainMenu != null)
            panelMainMenu.SetActive(true);
        if (panelHowToPlay != null)
            panelHowToPlay.SetActive(false);
    }

    public void OpenSettings()
    {
        if (panelSettings != null)
            panelSettings.SetActive(true);
        if (panelMainMenu != null)
            panelMainMenu.SetActive(false);
    }

    public  void CloseSettings()
    {
        if (panelSettings != null)
            panelSettings.SetActive(false);
        if (panelMainMenu != null)
            panelMainMenu.SetActive(true);
    }
}