using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace CoreAttack
{
    public class LiveScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreDisplay;

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

        private void UpdateDisplay(int score)
        {
            if (scoreDisplay != null)
            {
                scoreDisplay.SetText(score.ToString());
            }
        }
    }
}