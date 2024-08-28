using System.Collections;
using System.Collections.Generic;
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

    //Color Controller
    [SerializeField]
    private StateMachine colorsMachine;
    private LeakOMeterColorState redState, greenState, cyanState;

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

    private void Awake()
    {
        redState = new LeakOMeterColorState(gameObject, colorsMachine, Color.red, leakLamp);
        greenState = new LeakOMeterColorState(gameObject, colorsMachine, Color.green, leakLamp);
        cyanState = new LeakOMeterColorState(gameObject, colorsMachine, Color.cyan, leakLamp);
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

    private int trackedLeak = 0;
    private void UpdateBar(int leak)
    {
        float norm = (float)MLLeakTracker.Instance.LeakedMemory / MLLeakTracker.Instance.MaxMemory;
        if (leak - trackedLeak >= 0)
        {
            ChangeColor(redState);
            SetParticleEmission(true);
        }
        else
        {
            ChangeColor(greenState);
            SetParticleEmission(false);
        }
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

        trackedLeak = leak;
    }

    private void UpdateFrozenVisual(bool frozen)
    {
        if (frozen)
        {
            //if (leakLamp != null)
            //{
            //    leakLamp.color = Color.cyan;
            //}
            ChangeColor(cyanState);
            SetParticleEmission(false);
        }
        else
        {
            //if (leakLamp != null)
            //{
            //    leakLamp.color = Color.red;
            //}
            ChangeColor(redState);
            SetParticleEmission(true);
        }
    }

    private void SetParticleEmission(bool on)
    {
        if (psBorderLeak != null)
        {
            var em = psBorderLeak.emission;
            em.enabled = on;
        }
        if (psBodyLeak != null)
        {
            var em = psBodyLeak.emission;
            em.enabled = on;
        }
    }

    private void ChangeColor(LeakOMeterColorState colorState)
    {
        if (colorsMachine != null)
        {
            if (colorState != null)
            {
                colorsMachine.ChangeState(colorState);
            }
            else Debug.LogWarning($"ColorState in {name} is null");
        }
        else Debug.LogWarning($"ColorsMachine in {name} is null");
    }
}
