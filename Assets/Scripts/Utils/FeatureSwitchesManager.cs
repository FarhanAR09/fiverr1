using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeatureSwitchesManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform featureSwitchCanvas;
    [SerializeField]
    private Camera renderingCamera;
    private bool canvasEnabled = false;

    [SerializeField]
    private PurgeManager purgeManager;
    [SerializeField]
    private LevelManager levelManager;
    //Player Input

    //Toggles
    [SerializeField]
    private Toggle purgeToggle, speedOnFlushToggle, slowOnStopToggle, powerRequireChargeToggle, batteryCooldownChargeToggle, powersCooldownToggle;


    private void Awake()
    {
        //Canvas size
        if (featureSwitchCanvas != null)
        {
            featureSwitchCanvas.position = new Vector3(renderingCamera.transform.position.x, renderingCamera.transform.position.y, featureSwitchCanvas.transform.position.z);
            float height = 2f * renderingCamera.orthographicSize;
            float width = renderingCamera.aspect * height;
            featureSwitchCanvas.sizeDelta = new Vector2(width, height);

            featureSwitchCanvas.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (purgeToggle != null)
            purgeToggle.onValueChanged.AddListener(PurgeState);
        if (speedOnFlushToggle != null)
            speedOnFlushToggle.onValueChanged.AddListener(SpeedUpState);
        if (slowOnStopToggle != null)
            slowOnStopToggle.onValueChanged.AddListener(SlowDownState);
        if (powerRequireChargeToggle != null)
            powerRequireChargeToggle.onValueChanged.AddListener(RequireChargeState);
        if (batteryCooldownChargeToggle != null)
            batteryCooldownChargeToggle.onValueChanged.AddListener(CooldownBatteryState);
        if (powersCooldownToggle != null)
            powersCooldownToggle.onValueChanged.AddListener(PowersCooldownState);
    }

    private void OnDisable()
    {
        if (purgeToggle != null)
            purgeToggle.onValueChanged.RemoveListener(PurgeState);
        if (speedOnFlushToggle != null)
            speedOnFlushToggle.onValueChanged.RemoveListener(SpeedUpState);
        if (slowOnStopToggle != null)
            slowOnStopToggle.onValueChanged.RemoveListener(SlowDownState);
        if (powerRequireChargeToggle != null)
            powerRequireChargeToggle.onValueChanged.RemoveListener(RequireChargeState);
        if (batteryCooldownChargeToggle != null)
            batteryCooldownChargeToggle.onValueChanged.RemoveListener(CooldownBatteryState);
        if (powersCooldownToggle != null)
            powersCooldownToggle.onValueChanged.RemoveListener(PowersCooldownState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (featureSwitchCanvas != null)
            {
                if (canvasEnabled)
                {
                    featureSwitchCanvas.gameObject.SetActive(false);
                    canvasEnabled = false;
                    if (!GameSpeedManager.TryModifyGameSpeedModifier("FSPause", 1))
                        GameSpeedManager.TryAddGameSpeedModifier("FSPause", 1);
                }
                else
                {
                    featureSwitchCanvas.gameObject.SetActive(true);
                    canvasEnabled = true;
                    if (!GameSpeedManager.TryModifyGameSpeedModifier("FSPause", 0))
                        GameSpeedManager.TryAddGameSpeedModifier("FSPause", 0);
                }
            }
        }
    }

    private void PurgeState(bool state)
    {
        GameEvents.OnSwitchPurge.Publish(state);
    }

    private void SpeedUpState(bool state)
    {
        GameEvents.OnSwitchSpeedUp.Publish(state);
    }

    private void SlowDownState(bool state)
    {
        GameEvents.OnSwitchSlowDown.Publish(state);
    }

    private void RequireChargeState(bool state)
    {
        GameEvents.OnSwitchRequireCharge.Publish(state);
    }

    private void CooldownBatteryState(bool state)
    {
        GameEvents.OnSwitchCooldownCharge.Publish(state);
    }

    private void PowersCooldownState(bool state)
    {
        GameEvents.OnSwitchPowersCooldown.Publish(state);
    }
}
