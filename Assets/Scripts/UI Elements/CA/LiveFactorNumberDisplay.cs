using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CoreAttack
{
    public class LiveFactorNumberDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textDisplay;

        private void OnEnable()
        {
            if (FactorNumberTracker.Instance != null)
            {
                FactorNumberTracker.Instance.OnCounterUpdated += UpdateDisplay;
            }
        }

        private void OnDisable()
        {
            if (FactorNumberTracker.Instance != null)
            {
                FactorNumberTracker.Instance.OnCounterUpdated -= UpdateDisplay;
            }
        }

        private void Start()
        {
            if (FactorNumberTracker.Instance != null)
            {
                UpdateDisplay(FactorNumberTracker.Instance.Count);
            }
        }

        private void UpdateDisplay(int count)
        {
            if (textDisplay != null)
            {
                textDisplay.SetText(count.ToString());
            }
        }
    }
}