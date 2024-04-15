using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheStorage : MonoBehaviour
{
    [SerializeField]
    private int overflowChargeAmount = int.MaxValue;
    private int storedCache = 0;

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(TrackScore);
        GameEvents.OnLevelUp.Add(ResetStorage);
        
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(TrackScore);
        GameEvents.OnLevelUp.Remove(ResetStorage);
    }

    private void TrackScore(int addedScore)
    {
        storedCache += addedScore;
        if (storedCache >= overflowChargeAmount)
        {
            GameEvents.OnCacheOverflowed.Publish(true);
        }
    }

    private void ResetStorage(bool _)
    {
        storedCache = 0;
    }
}
