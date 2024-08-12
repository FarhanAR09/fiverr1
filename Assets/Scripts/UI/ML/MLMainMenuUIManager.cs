using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MLMainMenuUIManager : MonoBehaviour
{
    public static MLMainMenuUIManager Instance { get; private set; }

    [SerializeField]
    private GameObject mainMenuPanel, modeSelectionPanel, classicLevelSelectionPanel;

    [SerializeField]
    private List<Button> difficultiesButtons = new();
    [SerializeField]
    private List<TMP_Text> difficultiesButtonTexts = new();
    //[SerializeField]
    //private int unlockedLevel = 1;

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

    private void Start()
    {
        //Set button color by unlocked level in Classic mode
        if (difficultiesButtonTexts != null)
        {
            //int unlockedLevel = 3;
            int unlockedLevel = PlayerPrefs.GetInt(GameConstants.MLUNLOCKEDLEVEL, 1);
            for (int i = 0; i < difficultiesButtons.Count; i++)
            {
                difficultiesButtons[i].interactable = (i + 1) <= unlockedLevel;
                difficultiesButtonTexts[i].color = (i + 1) <= unlockedLevel ? Color.white : new Color(0.2f, 0.2f, 0.2f);
            }
        }

        //Enable only main panel
        SetPanel(mainMenuPanel, true);
        SetPanel(modeSelectionPanel, false);
        SetPanel(classicLevelSelectionPanel, false); ;
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    public void SwitchPanel(GameObject openedPanel, GameObject currentPanel)
    {
        SetPanel(currentPanel, false);
        SetPanel(openedPanel, true);
    }

    public void OpenModeSelection()
    {
        SetPanel(modeSelectionPanel, true);
        SetPanel(mainMenuPanel, false);
    }

    public void OpenClassicSelectLevel()
    {
        SetPanel(modeSelectionPanel, false);
        SetPanel(classicLevelSelectionPanel, true);
    }

    public void PlayClassic(int level)
    {
        PlayerPrefs.SetInt(GameConstants.MLLOADLEVEL, level);
        //print("Selected Level: " + level);
        SceneManager.LoadScene("MemoryLeakPlayScene");
    }

    public void PlayTrial()
    {
        //SceneManager.LoadScene();
    }

    public void PlayEndless()
    {
        //SceneManager.LoadScene();
    }

    public void BackToMainMenuPanel(GameObject currentPanel)
    {
        SwitchPanel(mainMenuPanel, currentPanel);
    }

    public void BackToModeSelection(GameObject currentPanel)
    {
        SwitchPanel(modeSelectionPanel, currentPanel);
    }
}
