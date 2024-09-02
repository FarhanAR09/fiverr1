using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class WorldCanvasFitterFollowFTCGameOver : WorldCanvasFitter
{
    private void OnEnable()
    {
        GameEvents.OnPlayerLose.Add(FitCanvasOnEvent);
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLose.Remove(FitCanvasOnEvent);
    }

    private void FitCanvasOnEvent(bool _)
    {
        FitCanvas();
    }
}
