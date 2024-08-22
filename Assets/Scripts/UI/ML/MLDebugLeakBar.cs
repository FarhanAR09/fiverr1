using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class MLDebugLeakBar : MonoBehaviour
{
    [SerializeField]
    private Transform cover;

    [SerializeField]
    private float height;

    [SerializeField]
    private ParticleSystem psBorderLeak, psBodyLeak;

    [SerializeField]
    private SpriteRenderer leakLamp;

    private float blinkFrequency = 4f;

    private Vector3 targetCoverPos, targetCoverScale, targetPsPos;

    private void OnEnable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated += UpdateBar;
        }
        else Debug.LogWarning("Tracker null");
        GameEvents.OnMLFreezeStateUpdated.Add(UpdateFrozenVisual);
    }

    private void OnDisable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated -= UpdateBar;
        }
        else Debug.LogWarning("Tracker null");
        GameEvents.OnMLFreezeStateUpdated.Remove(UpdateFrozenVisual);
    }

    private void Start()
    {
        UpdateBar(0);
    }

    private void Update()
    {
        if (leakLamp != null)
        {
            leakLamp.material.SetFloat("_Intensity", 2f + 1f * Mathf.Sin(blinkFrequency * Time.time));
        }
        float lerpAmount = 1f * Time.unscaledDeltaTime;
        if (cover != null)
        {
            cover.transform.localPosition = Vector3.Lerp(cover.transform.localPosition, targetCoverPos, lerpAmount);
            cover.localScale = Vector3.Lerp(cover.localScale, targetCoverScale, lerpAmount);
        }
        if (psBorderLeak != null)
        {
            psBorderLeak.transform.localPosition = Vector3.Lerp(psBorderLeak.transform.localPosition, targetPsPos, lerpAmount);
        }
    }

    private void UpdateBar(int _)
    {
        float norm = (float) MLLeakTracker.Instance.LeakedMemory / MLLeakTracker.Instance.MaxMemory;
        if (cover != null)
        {
            targetCoverPos = Vector3.Lerp(Vector3.zero, new Vector3(0, height / 2f, 0), norm);
            targetCoverScale = new Vector3(1, 1 - norm, 1);
        }
        if (psBorderLeak != null)
        {
            targetPsPos = new Vector3(0f, height * norm - height / 2f, 0f);
        }
        blinkFrequency = Mathf.Lerp(2f, 16f, norm);
        if (psBodyLeak != null)
        {
            psBodyLeak.transform.localPosition = new Vector3(0f, Mathf.Lerp(-height / 2f, 0f, norm), 0f);
            var shape = psBodyLeak.shape;
            shape.scale = new Vector3(0.78f, Mathf.Lerp(0f, height, norm), 1f);
            var em = psBodyLeak.emission;
            em.rateOverTime = Mathf.Lerp(4, 32, norm);
        }
    }

    private void UpdateFrozenVisual(bool frozen)
    {
        if (frozen)
        {
            if (leakLamp != null)
            {
                leakLamp.color = Color.cyan;
            }
            if (psBorderLeak != null)
            {
                var em = psBorderLeak.emission;
                em.enabled = false;
            }
            if (psBodyLeak != null)
            {
                var em = psBodyLeak.emission;
                em.enabled = false;
            }
        }
        else
        {
            if (leakLamp != null)
            {
                leakLamp.color = Color.red;
            }
            if (psBorderLeak != null)
            {
                var em = psBorderLeak.emission;
                em.enabled = true;
            }
            if (psBodyLeak != null)
            {
                var em = psBodyLeak.emission;
                em.enabled = true;
            }
        }
    }
}
