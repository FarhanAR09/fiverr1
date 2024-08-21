using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MLPlayCardNameDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textDisplay;

    private void OnEnable()
    {
        GameEvents.OnMLCardFlipped.Add(UpdateDisplay);
    }

    private void OnDisable()
    {
        
    }

    private void UpdateDisplay(CardFlipArgument arg)
    {
        if (textDisplay == null || !arg.isUp || arg.card == null || MLCardThemeManager.Instance == null)
            return;
        textDisplay.SetText(MLCardThemeManager.Instance.GetCardName(arg.card.CardNumber));
    }
}
