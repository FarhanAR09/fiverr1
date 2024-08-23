using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoCMainMenuManager : MonoBehaviour
{

    public static SoCMainMenuManager Instance { get; private set; }

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
    }

    public void LoadGame(int gameID)
    {
        SceneManager.LoadScene(gameID switch
        {
            0 => GameConstants.FTCMAINMENUSCENE,
            1 => GameConstants.MLMAINMENUSCENE,
            _ => GameConstants.SOCMAINMENUSCENE,
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
