using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI debugScoreDisplay;

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateScoreDisplay);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(UpdateScoreDisplay);
    }

    private void UpdateScoreDisplay(int score)
    {
        if (debugScoreDisplay != null)
            debugScoreDisplay.SetText("Score: " + score.ToString());
    }
}
