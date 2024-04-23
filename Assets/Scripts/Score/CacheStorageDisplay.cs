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

    [SerializeField]
    private ParticleSystem psFlush;

    /// <summary>
    /// Time after all gates collected and before leveling up
    /// </summary>
    private bool isFlushing = false;

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
        GameEvents.OnPurgeWarning.Add(GlowRedPurge);
        GameEvents.OnPurgeFinished.Add(GlowGreenPurge);
        GameEvents.OnAllGatesCollected.Add(FlushCache);

        GameEvents.OnAllGatesCollected.Add(StartFlushing);
        GameEvents.OnLevelUp.Add(FinishFlushing);
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
        GameEvents.OnPurgeWarning.Remove(GlowRedPurge);
        GameEvents.OnPurgeFinished.Remove(GlowGreenPurge);
        GameEvents.OnAllGatesCollected.Remove(FlushCache);

        GameEvents.OnAllGatesCollected.Remove(StartFlushing);
        GameEvents.OnLevelUp.Remove(FinishFlushing);
    }

    private void Update()
    {
        if (inPurge && glow != null)
        {
            glow.SetFloat("_Intensity", glowIntensity + 1f * Mathf.Sin(flickerFrequency * Time.time));
        }
    }

    private void UpdateDisplay(float addedScore)
    {
        if (cacheStorage != null)
        {
            if (!isFlushing)
            {
                normalizedCache = (cacheStorage.StoredCache + 10f) / cacheStorage.OverflowChargeAmount;
                if (addedScore < 0)
                {
                    StopCoroutine(FlashRedCorruption());
                    StartCoroutine(FlashRedCorruption());
                }
            }
            else
            {
                normalizedCache = 0f;
            }

            if (spriteRenderer != null && spriteRenderer.material != null)
            {
                spriteRenderer.material.SetFloat("_Reveal", normalizedCache);
            }
        }
        else Debug.LogWarning("CacheStorage is null");
    }

    private void GlowRedPurge(bool _)
    {
        StopCoroutine(FlashRedCorruption());
        inPurge = true;
        if (glow != null)
        {
            glow.SetColor("_Color", Color.red);
        }
    }

    private void GlowGreenPurge(bool _)
    {
        StopCoroutine(FlashRedCorruption());
        inPurge = false;
        if (glow != null)
        {
            glow.SetColor("_Color", glowColor);
            spriteRenderer.material.SetFloat("_Intensity", glowIntensity);
        }
    }

    private void FlushCache(bool _)
    {
        if (psFlush != null && cacheStorage != null)
        {
            psFlush.Emit(Mathf.CeilToInt(40f * Mathf.Pow(Mathf.Min(1f, (float)cacheStorage.StoredCache / cacheStorage.OverflowChargeAmount),2)));
        }
    }

    private void StartFlushing(bool _)
    {
        isFlushing = true;
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            spriteRenderer.material.SetFloat("_Reveal", 0f);
        }
    }

    private void FinishFlushing(bool _)
    {
        isFlushing = false;
    }

    private IEnumerator FlashRedCorruption()
    {
        if (glow != null)
            glow.SetColor("_Color", Color.red);
        yield return new WaitForSecondsRealtime(0.3f);
        if (glow != null)
            glow.SetColor("_Color", glowColor);
    }
}