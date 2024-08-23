using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MLPowerManager : MonoBehaviour
{
    public static MLPowerManager Instance { get; private set; }

    [SerializeField]
    private AudioClip sfxFlash, sfxFreezeStart, sfxFreezeEnd;

    private bool canFlash = false, flashUnlocked = true;
    private readonly float flashCdDuration = 90f;
    private float flashCdTime = 0f;
    private int flashLevel = 1;

    private bool canFreeze = false, isFrozen = false, freezeUnlocked = true;
    private readonly float freezeCdDuration = 45f, freezeDuration = 15f;
    private float freezeCdTime = 0f, freezeTime = 0f;

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

        flashLevel = PlayerPrefs.GetInt(GameConstants.MLUPGRADEFLASH, 1);
        flashUnlocked = flashLevel > 1;
        freezeUnlocked = PlayerPrefs.GetInt(GameConstants.MLUPGRADEFREEZE, 1) > 1;
    }

    private void Start()
    {
        ResetFlashCD();

        canFreeze = false;
        freezeCdTime = 0f;
        GameEvents.OnMLFreezeCDStarted.Publish(true);
    }

    private void Update()
    {
        if (flashUnlocked && canFlash && Input.GetKeyDown(KeyCode.Q))
        {
            ResetFlashCD();
            if (sfxFlash != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(sfxFlash, 100);
            }
            GameEvents.OnMLFlashPowerStarted.Publish(true);
        }

        if (freezeUnlocked && canFreeze && Input.GetKeyDown(KeyCode.E))
        {
            freezeTime = 0f;
            canFreeze = false;
            isFrozen = true;
            if (sfxFreezeStart != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(sfxFreezeStart, 100);
            }
            GameEvents.OnMLFreezeStateUpdated.Publish(true);
        }
    }

    private void FixedUpdate()
    {
        if (flashUnlocked)
        {
            if (!canFlash)
            {
                if (flashCdTime < flashCdDuration)
                {
                    flashCdTime += Time.fixedDeltaTime;
                    GameEvents.OnMLFlashCDUpdated.Publish(flashCdTime / flashCdDuration);
                }
                else
                {
                    canFlash = true;
                    GameEvents.OnMLFlashCDFinished.Publish(true);
                }
            }
        }

        if (freezeUnlocked)
        {
            //Frozen active countdown
            if (isFrozen)
            {
                if (freezeTime < freezeDuration)
                {
                    freezeTime += Time.fixedDeltaTime;
                    GameEvents.OnMLFreezeCDUpdated.Publish(1 - freezeTime / freezeDuration);
                }
                else
                {
                    isFrozen = false;
                    canFreeze = false;
                    freezeTime = 0f;
                    freezeCdTime = 0f;
                    if (sfxFreezeEnd != null && SFXController.Instance != null)
                    {
                        SFXController.Instance.RequestPlay(sfxFreezeEnd, 100);
                    }
                    GameEvents.OnMLFreezeStateUpdated.Publish(false);
                    GameEvents.OnMLFreezeCDStarted.Publish(true);
                }
            }
            else //Frozen Cooldown (when not frozen)
            {
                if (!canFreeze)
                {
                    if (freezeCdTime < freezeCdDuration)
                    {
                        freezeCdTime += Time.fixedDeltaTime;
                        GameEvents.OnMLFreezeCDUpdated.Publish(freezeCdTime / freezeCdDuration);
                    }
                    else
                    {
                        GameEvents.OnMLFreezeCDFinished.Publish(true);
                        canFreeze = true;
                    }
                }
            }
        }
    }

    private void ResetFlashCD()
    {
        canFlash = false;
        flashCdTime = 0f;
        GameEvents.OnMLFlashCDStarted.Publish(true);
    }
}
