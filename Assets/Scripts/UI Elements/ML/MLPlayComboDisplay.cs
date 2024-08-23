using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MLPlayComboDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textDisplay;

    private void OnEnable()
    {
        GameEvents.OnMLComboUpdated.Add(UpdateDisplay);
    }

    private void OnDisable()
    {
        GameEvents.OnMLComboUpdated.Remove(UpdateDisplay);
    }

    private void Start()
    {
        UpdateDisplay(0);
    }

    private void UpdateDisplay(int combo)
    {
        if (textDisplay != null)
        {
            textDisplay.SetText($"COMBO: {combo}");
        }
    }
}
