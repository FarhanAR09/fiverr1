using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.Rendering.DebugUI;
#endif

[DefaultExecutionOrder(-9999)]
public class PlaySceneManager : MonoBehaviour
{
    public static PlaySceneManager Instance { get; private set; }

    [SerializeField]
    private Transform LoseCanvas;
    [SerializeField]
    private GameObject panelLose, panelLeaderboardInput;
    [SerializeField]
    private TMP_InputField nameInput;
    [SerializeField]
    private AnimationCurve LoseAnimation;
    private readonly Vector2 StartLosePosition = new(14.4f, 32f), EndLosePosition = new(14.4f, 7.2f);
    private readonly float loseAnimationDuration = 1f;
    private float loseAnimationTime = 0;

    [SerializeField]
    private AudioClip combatMusic;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (LoseCanvas != null)
        {
            LoseCanvas.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLose.Add(Lose);
        GameEvents.OnPlayerLose.Add(KeepCredits);
    }

    private void Start()
    {
        ScoreCounter.ResetScore();
        if (LoseCanvas != null)
        {
            LoseCanvas.position = StartLosePosition;
        }

        if (panelLose != null)
        {
            panelLose.SetActive(true);
        }
        if (panelLeaderboardInput != null)
        {
            panelLeaderboardInput.SetActive(false);
        }

        if (MusicController.Instance != null && combatMusic != null)
        {
            MusicController.Instance.Stop();
            MusicController.Instance.SetClip(combatMusic);
            MusicController.Instance.Play();
        }
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLose.Remove(Lose);
        GameEvents.OnPlayerLose.Remove(KeepCredits);
    }

    private void OnDestroy()
    {
        ScoreCounter.ResetScore();
        StopAllCoroutines();
        ToMainMenu();
    }

    private void Lose(bool _)
    {
        SetHighscore();

        if (LoseCanvas != null)
        {
            LoseCanvas.gameObject.SetActive(true);

            bool newLeaderMade = LeaderboardDataManager.CheckLeaderboardEligibility(ScoreCounter.TotalScore);
            if (panelLose != null)
            {
                panelLose.SetActive(!newLeaderMade);
            }
            if (panelLeaderboardInput != null)
            {
                panelLeaderboardInput.SetActive(newLeaderMade);
            }

            if (LoseAnimation != null)
            {
                IEnumerator AnimationLoop()
                {
                    loseAnimationTime = 0;
                    while (true)
                    {
                        if (loseAnimationTime < loseAnimationDuration)
                        {
                            yield return new WaitForFixedUpdate();
                            loseAnimationTime += Time.fixedDeltaTime;

                            LoseCanvas.GetComponent<RectTransform>().position = Vector2.Lerp(EndLosePosition, StartLosePosition, LoseAnimation.Evaluate(loseAnimationTime / loseAnimationDuration));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                StartCoroutine(AnimationLoop());
            }
            else
            {
                LoseCanvas.position = EndLosePosition;
            }
        }
        else
        {
            ToMainMenu();
        }
    }

    public void ToMainMenu()
    {
        SetHighscore();
        ScoreCounter.ResetScore();
        SceneManager.LoadScene(GameConstants.FTCMAINMENUSCENE);
    }

    private void SetHighscore()
    {
        float currentHighscore = PlayerPrefs.HasKey("highscore") ? Mathf.Max(PlayerPrefs.GetFloat("highscore"), ScoreCounter.TotalScore) : 0;
        PlayerPrefs.SetFloat("highscore", currentHighscore);
        PlayerPrefs.Save();
    }

    public void UpdateLeaderboard()
    {
        string name = "Unnamed";
        if (nameInput != null && nameInput.text != "")
        {
            name = nameInput.text;
        }
        LeaderboardDataManager.TryAddToList(name, ScoreCounter.TotalScore);

        if (panelLose != null)
        {
            panelLose.SetActive(true);
        }
        if (panelLeaderboardInput != null)
        {
            panelLeaderboardInput.SetActive(false);
        }
    }

    private void KeepCredits(bool _)
    {
        CreditManager.LoadCredit(GameConstants.FTCCREDIT);
        CreditManager.DepositCredit(GameConstants.FTCCREDIT, ScoreCounter.TotalScore);
        CreditManager.SaveCredit(GameConstants.FTCCREDIT);
    }
}
