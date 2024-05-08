using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform canvasUI;
    [SerializeField]
    private Camera renderingCamera;

    [SerializeField]
    private Image empDisplay, bullettimeDisplay, boostDisplay;

    private void Awake()
    {
        if (canvasUI != null && renderingCamera != null)
        {
            canvasUI.position = new Vector3(renderingCamera.transform.position.x, renderingCamera.transform.position.y, canvasUI.transform.position.z);

            float height = 2f * renderingCamera.orthographicSize;
            float width = renderingCamera.aspect * height;
            canvasUI.sizeDelta = new Vector2(width, height);
        }
    }

    private void Start()
    {
        if (empDisplay != null)
        {
            empDisplay.material.SetColor("_Color", Color.white);
            empDisplay.material.SetFloat("_Intensity", 1f);
            empDisplay.material.SetFloat("_Reveal", 0f);
        }
        if (bullettimeDisplay != null)
        {
            bullettimeDisplay.material.SetColor("_Color", Color.white);
            bullettimeDisplay.material.SetFloat("_Intensity", 1f);
            bullettimeDisplay.material.SetFloat("_Reveal", 0f);
        }
        if (boostDisplay != null)
        {
            boostDisplay.material.SetColor("_Color", Color.white);
            boostDisplay.material.SetFloat("_Intensity", 1f);
            boostDisplay.material.SetFloat("_Reveal", 0f);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnEMPCooldownUpdated.Add(UpdateEMP);
        GameEvents.OnEMPCooldownStarted.Add(ResetEMP);
        GameEvents.OnEMPCooldownFinished.Add(IntensifyEMP);

        GameEvents.OnBulletTimeCooldownUpdated.Add(UpdateBulletTime);
        GameEvents.OnBulletTimeCooldownStarted.Add(ResetBulletTime);
        GameEvents.OnBulletTimeCooldownFinished.Add(IntensifyBulletTime);

        GameEvents.OnBoostCooldownUpdated.Add(UpdateBoost);
        GameEvents.OnBoostCooldownStarted.Add(ResetBoost);
        GameEvents.OnBoostCooldownFinished.Add(IntensifyBoost);
    }

    private void OnDisable()
    {
        GameEvents.OnEMPCooldownUpdated.Remove(UpdateEMP);
        GameEvents.OnEMPCooldownStarted.Remove(ResetEMP);
        GameEvents.OnEMPCooldownFinished.Remove(IntensifyEMP);

        GameEvents.OnBulletTimeCooldownUpdated.Remove(UpdateBulletTime);
        GameEvents.OnBulletTimeCooldownStarted.Remove(ResetBulletTime);
        GameEvents.OnBulletTimeCooldownFinished.Remove(IntensifyBulletTime);

        GameEvents.OnBoostCooldownUpdated.Remove(UpdateBoost);
        GameEvents.OnBoostCooldownStarted.Remove(ResetBoost);
        GameEvents.OnBoostCooldownFinished.Remove(IntensifyBoost);
    }

    private void UpdateEMP(float progress)
    {
        //Debug.Log("EMP Progress Update: " + progress);
        if (empDisplay != null)
        {
            UpdateDisplay(empDisplay, progress);
        }
        else Debug.LogWarning("No EMP Cooldown Display!");
    }

    private void ResetEMP(bool _)
    {
        //Debug.Log("EMP Intensity Reset");
        if (empDisplay != null)
        {
            ResetDisplay(empDisplay);
        }
        else Debug.LogWarning("No EMP Cooldown Display!");
    }

    private void IntensifyEMP(bool _)
    {
        //Debug.Log("EMP Intensified");
        if (empDisplay != null)
        {
            IntensifyDisplay(empDisplay);
        }
        else Debug.LogWarning("No EMP Cooldown Display!");
    }

    private void UpdateBulletTime(float progress)
    {
        //Debug.Log("Bullet Time Progress Update: " + progress);
        if (bullettimeDisplay != null)
        {
            UpdateDisplay(bullettimeDisplay, progress);
        }
        else Debug.LogWarning("No Bullet Time Cooldown Display!");
    }

    private void ResetBulletTime(bool _)
    {
        //Debug.Log("Bullet Time Intensity Reset");
        if (bullettimeDisplay != null)
        {
            ResetDisplay(bullettimeDisplay);
        }
        else Debug.LogWarning("No Bullet Time Cooldown Display!");
    }

    private void IntensifyBulletTime(bool _)
    {
        //Debug.Log("Bullet Time Intensified");
        if (bullettimeDisplay != null)
        {
            IntensifyDisplay(bullettimeDisplay);
        }
        else Debug.LogWarning("No Bullet Time Cooldown Display!");
    }

    private void UpdateBoost(float progress)
    {
        //Debug.Log("Boost Progress Update: " + progress);
        if (boostDisplay != null)
        {
            UpdateDisplay(boostDisplay, progress);
        }
        else Debug.LogWarning("No Boost Cooldown Display!");
    }

    private void ResetBoost(bool _)
    {
        //Debug.Log("Boost Intensity Reset");
        if (boostDisplay != null)
        {
            ResetDisplay(boostDisplay);
        }
        else Debug.LogWarning("No Boost Cooldown Display!");
    }

    private void IntensifyBoost(bool _)
    {
        //Debug.Log("Boost Intensified");
        if (boostDisplay != null)
        {
            IntensifyDisplay(boostDisplay);
        }
        else Debug.LogWarning("No Boost Cooldown Display!");
    }

    private void UpdateDisplay(Image display, float progress)
    {
        display.material.SetFloat("_Reveal", progress);
        display.material.SetFloat("_Intensity", 5f * progress);
        display.color = Color.HSVToRGB(0.5f * progress, 1f, 0.25f);
    }

    private void ResetDisplay(Image display)
    {
        display.material.SetFloat("_Intensity", 0f);
    }

    private void IntensifyDisplay(Image display)
    {
        display.material.SetFloat("_Intensity", 7f);
    }
}