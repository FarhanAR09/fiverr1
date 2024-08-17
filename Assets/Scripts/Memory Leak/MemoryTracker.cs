using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryTracker : MonoBehaviour
{
    public static MemoryTracker Instance { get; private set; }

    public int Combo { get; private set; } = 0;
    public int Mistakes { get; private set; } = 0;

    private void OnEnable()
    {
        GameEvents.OnMLCardsPaired.Add(TrackSuccessfulPairings);
        GameEvents.OnMLCardsFailPairing.Add(TrackFailedPairings);
        GameEvents.OnMLCardFinishedSingleCheck.Add(SingleCheckBreakCombo);
    }

    private void OnDisable()
    {
        GameEvents.OnMLCardsPaired.Remove(TrackSuccessfulPairings);
        GameEvents.OnMLCardsFailPairing.Remove(TrackFailedPairings);
        GameEvents.OnMLCardFinishedSingleCheck.Remove(SingleCheckBreakCombo);
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

        Mistakes = 0;
        Combo = 0;
    }

    private void TrackPairings(bool success)
    {
        if (success)
        {
            Combo++;
            GameEvents.OnMLComboUpdated.Publish(Combo);
        }
        else
        {
            if (Combo > 0)
                BreakCombo();
            Mistakes++;
            GameEvents.OnMLMistakesUpdated.Publish(Mistakes);
        }
    }

    private void TrackSuccessfulPairings(CardPairArgument arg)
    {
        TrackPairings(true);
    }

    private void TrackFailedPairings(CardPairArgument arg)
    {
        TrackPairings(false);
    }

    private void BreakCombo()
    {
        Combo = 0;
        GameEvents.OnMLComboBroken.Publish(true);
        GameEvents.OnMLComboUpdated.Publish(Combo);
    }

    private void SingleCheckBreakCombo(RAMCard _)
    {
        if (Combo > 0)
            BreakCombo();
    }
}
