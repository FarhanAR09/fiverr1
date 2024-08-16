using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PowerCooldownDisplay : MonoBehaviour
{
    private Image image;
    [SerializeField]
    private PowerTypes powerType = PowerTypes.EMP;

    private bool unlocked = false;

    private void Awake()
    {
        TryGetComponent(out image);

        unlocked = PlayerPrefs.GetInt(GetUpgradeKeyByPowerType(), 1) > 1;
    }

    private void Start()
    {
        if (image != null)
        {
            image.material.SetColor("_Color", Color.white);
            image.material.SetFloat("_Intensity", 1f);
            image.material.SetFloat("_Reveal", 0f);
        }
    }

    private void OnEnable()
    {
        if (unlocked)
        {
            switch (powerType)
            {
                //FTC
                case PowerTypes.EMP:
                    GameEvents.OnEMPCooldownUpdated.Add(UpdateDisplayFromNormalized);
                    GameEvents.OnEMPCooldownStarted.Add(ResetDisplay);
                    GameEvents.OnEMPCooldownFinished.Add(IntensifyDisplay);
                    break;
                case PowerTypes.BulletTime:
                    GameEvents.OnBulletTimeCooldownUpdated.Add(UpdateDisplayFromNormalized);
                    GameEvents.OnBulletTimeCooldownStarted.Add(ResetDisplay);
                    GameEvents.OnBulletTimeCooldownFinished.Add(IntensifyDisplay);
                    break;
                case PowerTypes.Boost:
                    GameEvents.OnBoostCooldownUpdated.Add(UpdateDisplayFromNormalized);
                    GameEvents.OnBoostCooldownStarted.Add(ResetDisplay);
                    GameEvents.OnBoostCooldownFinished.Add(IntensifyDisplay);
                    break;

                //ML
                case PowerTypes.Flash:
                    GameEvents.OnMLFlashCDUpdated.Add(UpdateDisplayFromNormalized);
                    GameEvents.OnMLFlashCDStarted.Add(ResetDisplay);
                    GameEvents.OnMLFlashCDFinished.Add(IntensifyDisplay);
                    break;
                case PowerTypes.Freeze:
                    GameEvents.OnMLFreezeCDUpdated.Add(UpdateDisplayFromNormalized);
                    GameEvents.OnMLFreezeCDStarted.Add(ResetDisplay);
                    GameEvents.OnMLFreezeCDFinished.Add(IntensifyDisplay);
                    break;
            }
        }
    }

    private void OnDisable()
    {
        if (unlocked)
        {
            switch (powerType)
            {
                //FTC
                case PowerTypes.EMP:
                    GameEvents.OnEMPCooldownUpdated.Remove(UpdateDisplayFromNormalized);
                    GameEvents.OnEMPCooldownStarted.Remove(ResetDisplay);
                    GameEvents.OnEMPCooldownFinished.Remove(IntensifyDisplay);
                    break;
                case PowerTypes.BulletTime:
                    GameEvents.OnBulletTimeCooldownUpdated.Remove(UpdateDisplayFromNormalized);
                    GameEvents.OnBulletTimeCooldownStarted.Remove(ResetDisplay);
                    GameEvents.OnBulletTimeCooldownFinished.Remove(IntensifyDisplay);
                    break;
                case PowerTypes.Boost:
                    GameEvents.OnBoostCooldownUpdated.Remove(UpdateDisplayFromNormalized);
                    GameEvents.OnBoostCooldownStarted.Remove(ResetDisplay);
                    GameEvents.OnBoostCooldownFinished.Remove(IntensifyDisplay);
                    break;

                //ML
                case PowerTypes.Flash:
                    GameEvents.OnMLFlashCDUpdated.Remove(UpdateDisplayFromNormalized);
                    GameEvents.OnMLFlashCDStarted.Remove(ResetDisplay);
                    GameEvents.OnMLFlashCDFinished.Remove(IntensifyDisplay);
                    break;
                case PowerTypes.Freeze:
                    GameEvents.OnMLFreezeCDUpdated.Remove(UpdateDisplayFromNormalized);
                    GameEvents.OnMLFreezeCDStarted.Remove(ResetDisplay);
                    GameEvents.OnMLFreezeCDFinished.Remove(IntensifyDisplay);
                    break;
            }
        }
    }

    private void UpdateDisplayFromNormalized(float value)
    {
        if (!unlocked) return;
        if (image != null)
        {
            image.material.SetFloat("_Reveal", value);
            image.material.SetFloat("_Intensity", 5f * value);
            image.color = Color.HSVToRGB(0.5f * value, 1f, 0.25f);
        }
        else Debug.LogWarning("No Image Component");
    }

    private void ResetDisplay(bool _)
    {
        if (!unlocked) return;
        print(name + " reset");
        if (image != null)
        {
            image.material.SetFloat("_Intensity", 0f);
        }
        else Debug.LogWarning("No Image Component");
    }

    private void IntensifyDisplay(bool _)
    {
        if (!unlocked) return;
        print(name + " full");
        if (image != null)
        {
            image.material.SetFloat("_Intensity", 7f);
        }
        else Debug.LogWarning("No Image Component");
    }

    private string GetUpgradeKeyByPowerType()
    {
        return powerType switch
        {
            PowerTypes.EMP => GameConstants.FTCUPGRADEEMP,
            PowerTypes.BulletTime => GameConstants.FTCUPGRADEBULLETTIME,
            PowerTypes.Boost => GameConstants.FTCUPGRADEBOOST,

            PowerTypes.Flash => GameConstants.MLUPGRADEFLASH,
            PowerTypes.Freeze => GameConstants.MLUPGRADEFREEZE,

            _ => GameConstants.FTCUPGRADEEMP
        };
    }
}
