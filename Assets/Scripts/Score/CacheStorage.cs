using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheStorage : MonoBehaviour
{
    public static CacheStorage Instance { get; private set; }

    [field: SerializeField]
    public float OverflowChargeAmount { get; private set; } = int.MaxValue;
    public float StoredCache { get; private set; } = 0;

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
    }

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(TrackScore);
        GameEvents.OnLevelUp.Add(ResetStorage);
        GameEvents.OnUpdateCacheOverflow.Add(SetOverflowAmount);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(TrackScore);
        GameEvents.OnLevelUp.Remove(ResetStorage);
        GameEvents.OnUpdateCacheOverflow.Remove(SetOverflowAmount);
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

    private void SetOverflowAmount(int amount)
    {
        OverflowChargeAmount = amount;
    }
}
