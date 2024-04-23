using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
#endif

[DefaultExecutionOrder(-9999)]
public class PlaySceneManager : MonoBehaviour
{
    public static PlaySceneManager Instance { get; private set; }

    [SerializeField]
    private Transform LoseCanvas;
    [SerializeField]
    private AnimationCurve LoseAnimation;
    private readonly Vector2 StartLosePosition = new(14.4f, 32f), EndLosePosition = new(14.4f, 7.2f);
    private readonly float loseAnimationDuration = 1f;
    private float loseAnimationTime = 0;

    private void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLose.Add(Lose);
    }

    private void Start()
    {
        ScoreCounter.ResetScore();
        if (LoseCanvas != null)
        {
            LoseCanvas.position = StartLosePosition;
        }

        if (MusicController.instance != null)
        {
            MusicController.instance.Stop();
            MusicController.instance.Play();
        }
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLose.Remove(Lose);
    }

    private void OnDestroy()
    {
        ScoreCounter.ResetScore();
        StopAllCoroutines();
        ToMainMenu();
    }

    private void Lose(bool enabled)
    {
        if (!enabled)
        {
            SetHighscore();

            if (LoseCanvas != null)
            {
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
    }

    public void ToMainMenu()
    {
        SetHighscore();
        ScoreCounter.ResetScore();
        SceneManager.LoadScene("MainMenuScene");
    }

    private void SetHighscore()
    {
        float currentHighscore = PlayerPrefs.HasKey("highscore") ? Mathf.Max(PlayerPrefs.GetFloat("highscore"), ScoreCounter.Score) : 0;
        PlayerPrefs.SetFloat("highscore", currentHighscore);
        PlayerPrefs.Save();
    }
}