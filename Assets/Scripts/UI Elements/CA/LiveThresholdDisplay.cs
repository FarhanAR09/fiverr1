using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CoreAttack
{
    public class LiveThresholdDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textDisplay, hzDisplay;

        private void OnEnable()
        {
            if (Threshold.Instance != null)
            {
                Threshold.Instance.OnThresholdUpdated += UpdateDisplay;
                Threshold.Instance.OnLimitReached += TurnGreen;
            }
        }

        private void OnDisable()
        {
            if (Threshold.Instance != null)
            {
                Threshold.Instance.OnThresholdUpdated -= UpdateDisplay;
                Threshold.Instance.OnLimitReached -= TurnGreen;
            }
        }

        private void Start()
        {
            if (Threshold.Instance != null)
            {
                UpdateDisplay(Threshold.Instance.Amount);
            }
        }

        private void UpdateDisplay(float threshold)
        {
            if (textDisplay != null)
            {
                if (Threshold.Instance != null)
                {
                    if (Threshold.Instance.Amount < Threshold.Instance.Limit)
                    {
                        textDisplay.SetText($"{threshold:F0}/{Threshold.Instance.Limit:F0}");
                    }
                    else
                    {
                        textDisplay.SetText($"{threshold:F0}");
                    }
                }
            }
            else Debug.LogWarning("Text Display is null");
        }

        private void TurnGreen()
        {
            if (textDisplay != null)
            {
                textDisplay.color = Color.green;
            }
            if (hzDisplay != null)
            {
                hzDisplay.color = Color.green;
            }
        }
    }
}