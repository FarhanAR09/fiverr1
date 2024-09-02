using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class WorldCanvasFitterFollowMLGameOver : WorldCanvasFitter
{
    private void OnEnable()
    {
        GameEvents.OnMLGameFinished.Add(FitCanvasOnEvent);
        GameEvents.OnMLLost.Add(FitCanvasOnEvent);
    }

    private void OnDisable()
    {
        GameEvents.OnMLGameFinished.Remove(FitCanvasOnEvent);
        GameEvents.OnMLLost.Remove(FitCanvasOnEvent);
    }

    private void FitCanvasOnEvent(bool _)
    {
        FitCanvas();
    }
}
