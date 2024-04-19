using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CacheStorageDisplay : MonoBehaviour
{
    [SerializeField]
    private CacheStorage cacheStorage;

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Color glowColor = Color.magenta;
    [SerializeField]
    private float glowIntensity = 8;

    private float normalizedCache = 0;

    private void Awake()
    {
        if (TryGetComponent(out SpriteRenderer sr))
        {
            spriteRenderer = sr;
        }
    }

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateDisplay);
        GameEvents.OnCacheOverflowed.Add(HandleOverflow);
    }


    private void Start()
    {
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            spriteRenderer.material.SetColor("_Color", glowColor);
            spriteRenderer.material.SetFloat("_Intensity", glowIntensity);
        }
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
            //Debug.Log($"Cache Display Normalized: {normalizedCache}");
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat("_Reveal", normalizedCache);
            }
        }
        else Debug.LogWarning("CacheStorage is null");
    }

    private void HandleOverflow(bool _)
    {
        Debug.Log("--------------------------------------------\n!!!!!!!!!!!!!!  Cache Overflow  !!!!!!!!!!!!\n--------------------------------------------");
    }
}