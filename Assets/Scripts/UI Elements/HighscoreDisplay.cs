using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class HighscoreDisplay : MonoBehaviour
{
    private TMP_Text display;

    private void Start()
    {
        TryGetComponent(out display);
        UpdateDisplay();
    }
    
    private void UpdateDisplay()
    {
        if (display != null)
        {
            display.SetText($"HIGHSCORE: {PlayerPrefs.GetFloat(GameConstants.MLHIGHSCORE, 0f):F0}");
        }
    }
}
