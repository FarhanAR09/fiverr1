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
    private GameObject panelMainMenu, panelLeaderboard;
    [SerializeField]
    private TextMeshProUGUI leaderNamesDisplay, leaderScoresDisplay;

    private void Start()
    {
        float highscore = PlayerPrefs.HasKey("highscore") ?
            PlayerPrefs.GetFloat("highscore") :
            0;
        if (highscoreDisplay != null)
            highscoreDisplay.SetText($"HIGHSCORE: {string.Format("{0:0.##}", highscore)}");

        if (MusicController.instance != null)
        {
            MusicController.instance.Stop();
            MusicController.instance.Play();
        }

        if (panelMainMenu != null)
        {
            panelMainMenu.SetActive(true);
        }
        if (panelLeaderboard != null)
        {
            panelLeaderboard.SetActive(false);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene("PLayScene");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void CloseLeaderboard()
    {
        panelMainMenu.SetActive(true);
        panelLeaderboard.SetActive(false);
    }

    public void OpenLeaderboard()
    {
        panelMainMenu.SetActive(false);
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

    public void OpenSettings()
    {
        Debug.LogWarning("Settings Unimplemented");
    }
}