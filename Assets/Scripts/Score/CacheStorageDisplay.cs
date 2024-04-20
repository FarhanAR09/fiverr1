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

    private Material glow;
    private bool inPurge = false;
    [SerializeField]
    private float flickerFrequency = 4;

    private void Awake()
    {
        if (TryGetComponent(out SpriteRenderer sr))
        {
            spriteRenderer = sr;
            if (sr.material != null)
            {
                glow = sr.material;
            }
        }
    }

    private void OnEnable()
    {
        ScoreCounter.OnScoreUpdated.AddListener(UpdateDisplay);
        GameEvents.OnPurgeStarted.Add(GlowRedPurge);
        GameEvents.OnPurgeFinished.Add(GlowGreenPurge);
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
        GameEvents.OnPurgeStarted.Remove(GlowRedPurge);
        GameEvents.OnPurgeFinished.Remove(GlowGreenPurge);
    }

    private void Update()
    {
        if (inPurge && glow != null)
        {
            glow.SetFloat("_Intensity", glowIntensity + 1f * Mathf.Sin(flickerFrequency * Time.time));
        }
    }

    private void UpdateDisplay(int _)
    {
        if (cacheStorage != null)
        {
            normalizedCache = (cacheStorage.StoredCache + 10f) / cacheStorage.OverflowChargeAmount;
            //Debug.Log($"Cache Display Normalized: {normalizedCache}");
            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat("_Reveal", normalizedCache);
            }
        }
        else Debug.LogWarning("CacheStorage is null");
    }

    private void GlowRedPurge(bool _)
    {
        inPurge = true;
        if (glow != null)
        {
            glow.SetColor("_Color", Color.red);
        }
    }

    private void GlowGreenPurge(bool _)
    {
        inPurge = false;
        if (glow != null)
        {
            glow.SetColor("_Color", glowColor);
            spriteRenderer.material.SetFloat("_Intensity", glowIntensity);
        }
    }
}