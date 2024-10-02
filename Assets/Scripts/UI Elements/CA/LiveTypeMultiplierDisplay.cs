using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CoreAttack
{
    public class LiveTypeMultiplierDisplay : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textDisplay;

        private void OnEnable()
        {
            GameEvents.OnCAEnemyDeath.Add(UpdateDisplay);
        }

        private void OnDisable()
        {
            GameEvents.OnCAEnemyDeath.Remove(UpdateDisplay);
        }

        private void UpdateDisplay(Enemy enemy)
        {
            if (textDisplay != null)
            {
                if (ScoreController.Instance != null)
                {
                    textDisplay.SetText(ScoreController.Instance.TypeToMultiplier(enemy.Type).ToString("F1"));
                }
            }
            else Debug.LogWarning("Text Display is null");
        }
    }
}