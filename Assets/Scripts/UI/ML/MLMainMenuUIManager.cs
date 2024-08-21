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
    private GameObject mainMenuPanel, modeSelectionPanel, classicLevelSelectionPanel, upgradePanel, themePanel;

    [SerializeField]
    private List<Button> difficultiesButtons = new();
    [SerializeField]
    private List<TMP_Text> difficultiesButtonTexts = new();
    //[SerializeField]
    //private int unlockedLevel = 1;
    [SerializeField]
    private TMP_Text upgradeCreditDisplay;

    private int cardThemeIndex = 0;

    [Tooltip("Sort according to ThemeID")]
    [SerializeField]
    private List<Toggle> themeToggles;
    [SerializeField]
    private ToggleGroup themeToggleGroup;

    private void OnEnable()
    {
        GameEvents.OnCreditUpdated.Add(UpdateUpgradeCreditDisplay);
    }

    private void OnDisable()
    {
        GameEvents.OnCreditUpdated.Remove(UpdateUpgradeCreditDisplay);
    }

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
        //Enable only main panel
        SetPanel(mainMenuPanel, true);
        SetPanel(modeSelectionPanel, false);
        SetPanel(classicLevelSelectionPanel, false);
        SetPanel(upgradePanel, false);
        SetPanel(themePanel, false);

        //Give credit on load
        //CreditManager.LoadCredit(GameConstants.MLCREDIT);
        //CreditManager.DepositCredit(GameConstants.MLCREDIT, 1000f);
        //CreditManager.SaveCredit(GameConstants.MLCREDIT);
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    public void OpenPanel(GameObject panel)
    {
        SetPanel(panel, true);
    }

    public void ClosePanel(GameObject panel)
    {
        SetPanel(panel, false);
    }

    public void LoadPlayScene()
    {
        static IEnumerator WaitAFrameBeforeLoading()
        {
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene("MemoryLeakPlayScene");
        }
        StopCoroutine(WaitAFrameBeforeLoading());
        StartCoroutine(WaitAFrameBeforeLoading());
    }

    public void UpdateUpgradeCreditDisplay(float _ = 0f)
    {
        CreditManager.LoadCredit(GameConstants.MLCREDIT);
        //print($"ML CREDIT: {CreditManager.GetCredit(GameConstants.MLCREDIT):F0}");
        if (upgradeCreditDisplay != null)
        {
            upgradeCreditDisplay.SetText($"CREDIT: {CreditManager.GetCredit(GameConstants.MLCREDIT):F0}");
        }
    }

    public void UpdateDifficultySelectionPage()
    {
        if (classicLevelSelectionPanel != null)
        {
            //Set button color by unlocked level in Classic mode
            if (difficultiesButtons != null && difficultiesButtonTexts != null)
            {
                //int unlockedLevel = 3;
                int unlockedLevel = PlayerPrefs.GetInt(GameConstants.MLUPGRADEDIFFICULTIES, 1);
                for (int i = 0; i < difficultiesButtons.Count; i++)
                {
                    difficultiesButtons[i].interactable = (i + 1) <= unlockedLevel;
                    difficultiesButtonTexts[i].color = (i + 1) <= unlockedLevel ? Color.white : new Color(0.2f, 0.2f, 0.2f);
                }
            }
        }
    }

    public void UpdateThemeSelectionPage()
    {
        if (themeToggleGroup && themeToggles != null && themeToggles.Count > 0)
        {
            int selectedTheme = PlayerPrefs.GetInt(GameConstants.MLSELECTCARDTHEMEINDEX, 0);
            themeToggles[selectedTheme].isOn = true;
        }
    }

    public void SelectMode(int mode)
    {
        PlayerPrefs.SetInt(GameConstants.MLLOADGAMEMODE, mode);
    }

    public void SelectDifficulty(int difficulty)
    {
        PlayerPrefs.SetInt(GameConstants.MLLOADLEVEL, Mathf.Clamp(difficulty, 1, 5));
    }

    public void SelectCardTheme(int cardThemeIndex)
    {
        if (cardThemeIndex == this.cardThemeIndex)
            return;
        this.cardThemeIndex = cardThemeIndex;
        PlayerPrefs.SetInt(GameConstants.MLSELECTCARDTHEMEINDEX, cardThemeIndex);
    }
}
