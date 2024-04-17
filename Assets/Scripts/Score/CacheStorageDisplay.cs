using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CacheStorageDisplay : MonoBehaviour
{
    [SerializeField]
    private CacheStorage cacheStorage;

    private float normalizedCache = 0;

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateDisplay);
        GameEvents.OnCacheOverflowed.Add(HandleOverflow);
    }


    private void Start()
    {
        UpdateDisplay(0);
    }

    private void OnDisable()
    {
        ScoreCounter.OnScoreUpdated.RemoveListener(UpdateDisplay);
        GameEvents.OnCacheOverflowed.Remove(HandleOverflow);
    }

    private void UpdateDisplay(int _)
    {
        if (cacheStorage != null)
        {
            normalizedCache = (float) cacheStorage.StoredCache / cacheStorage.OverflowChargeAmount;
            Debug.Log($"Cache Display Normalized: {normalizedCache}");
        }
        else Debug.LogWarning("CacheStorage is null");
    }

    private void HandleOverflow(bool _)
    {
        Debug.Log("--------------------------------------------");
        Debug.Log("!!!!!!!!!!!!!!  Cache Overflow  !!!!!!!!!!!!");
        Debug.Log("--------------------------------------------");
    }
}
