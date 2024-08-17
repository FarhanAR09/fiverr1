using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MLFinishScreenManager : MonoBehaviour
{
    public static MLFinishScreenManager Instance { get; private set; }

    [Tooltip("Must be full size on canvas")]
    [SerializeField]
    private RectTransform finishPanel;
    [SerializeField]
    private TMP_Text finalScoreDisplay;

    private float screenHeight, screenWidth;

    private void OnEnable()
    {
        GameEvents.OnMLLost.Add(ShowFinishScreen);
        GameEvents.OnMLAllCardsPaired.Add(ShowFinishScreen);

        GameEvents.OnMLLost.Add(AddCredit);
        GameEvents.OnMLAllCardsPaired.Add(AddCredit);
    }

    private void OnDisable()
    {
        GameEvents.OnMLLost.Remove(ShowFinishScreen);
        GameEvents.OnMLAllCardsPaired.Remove(ShowFinishScreen);

        GameEvents.OnMLLost.Remove(AddCredit);
        GameEvents.OnMLAllCardsPaired.Remove(AddCredit);
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
        
        UpdatePanelSize();
    }

    private void Start()
    {
        if (finishPanel != null)
        {
            finishPanel.sizeDelta = new Vector2(screenWidth, screenHeight);
            finishPanel.localPosition = new Vector3(0f, screenHeight);
            finishPanel.gameObject.SetActive(false);
        }
    }

    private void UpdatePanelSize()
    {
        screenHeight = Camera.main.orthographicSize * 2;
        screenWidth = screenHeight * Camera.main.aspect;
    }

    private void ShowFinishScreen(bool _)
    {
        if (finishPanel != null)
        {
            finishPanel.gameObject.SetActive(true);
            //finishPanel.localPosition = Vector2.zero;
            finishPanel.offsetMin = Vector2.zero;
            finishPanel.offsetMax = Vector2.zero;
        }

        if (finalScoreDisplay != null)
        {
            float rating = CardMatchController.Instance != null ?
                Mathf.Ceil(10f * CardMatchController.Instance.PairCount / CardMatchController.Instance.MaxPairCount * Mathf.Exp(-0.0025f * CardMatchController.Instance.TimesCardOpened)) / 2f :
                0;
            finalScoreDisplay.SetText($"SCORE:\t{(MLScoreManager.Instance != null ? MLScoreManager.Instance.Score.ToString("F0") : 0)}\r\nRATING:\t{rating:F1}/5.0");
        }
    }

    private void AddCredit(bool _)
    {
        float fullCredit = 1000;
        if (MLPlayManager.Instance != null)
        {
            //Full Credit = 1000 * (cardCount / 16)^2
            fullCredit = MLPlayManager.Instance.Difficulty switch
            {
                1 => 1000,
                2 => 1500,
                3 => 3500,
                4 => 5000,
                5 => 7000,
                _ => 1000
            };
        }
        float addedCredit = fullCredit * CardMatchController.Instance.PairCount / CardMatchController.Instance.MaxPairCount;
        CreditManager.DepositCredit(GameConstants.MLCREDIT, addedCredit);
        CreditManager.SaveCredit(GameConstants.MLCREDIT);
    }
}
