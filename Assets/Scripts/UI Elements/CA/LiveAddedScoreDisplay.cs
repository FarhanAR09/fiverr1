using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CoreAttack
{
    public class LiveAddedScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textDisplay;

        private int previousScore = 0;

        private void OnEnable()
        {
            if (Score.Instance != null)
            {
                Score.Instance.OnScoreUpdated += UpdateDisplay;
            }
        }

        private void OnDisable()
        {
            if (Score.Instance != null)
            {
                Score.Instance.OnScoreUpdated -= UpdateDisplay;
            }
        }

        private void Start()
        {
            if (Score.Instance != null)
            {
                previousScore = Score.Instance.Amount;
            }
        }

        private void UpdateDisplay(int score)
        {
            if (textDisplay != null)
            {
                textDisplay.SetText((score - previousScore).ToString());
            }
        }
    }
}