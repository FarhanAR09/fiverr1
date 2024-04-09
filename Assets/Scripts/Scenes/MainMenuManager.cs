using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI highscoreDisplay;

    private void Start()
    {
        int highscore = PlayerPrefs.HasKey("highscore") ?
            PlayerPrefs.GetInt("highscore") :
            0;
        if (highscoreDisplay != null)
            highscoreDisplay.SetText($"HIGHSCORE: {highscore}");

        if (MusicController.instance != null)
        {
            MusicController.instance.Stop();
            MusicController.instance.Play();
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
}