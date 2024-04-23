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