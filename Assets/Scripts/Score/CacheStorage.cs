using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheStorage : MonoBehaviour
{
    [field: SerializeField]
    public float OverflowChargeAmount { get; private set; } = int.MaxValue;
    public float StoredCache { get; private set; } = 0;

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

    private void TrackScore(float addedScore)
    {
        //Debug.Log($"Added score: {addedScore}");
        StoredCache += addedScore;
        //Debug.Log($"Stored score: {StoredCache}");
        if (StoredCache >= OverflowChargeAmount)
        {
            GameEvents.OnCacheOverflowed.Publish(true);
        }
    }

    private void ResetStorage(bool _)
    {
        StoredCache = 0f;
    }
}
