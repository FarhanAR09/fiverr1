using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MLPowerManager : MonoBehaviour
{
    public static MLPowerManager Instance { get; private set; }

    private bool canFlash = false, flashUnlocked = true;
    private readonly float flashCdDuration = 30f;
    private float flashCdTime = 0f;

    private bool canFreeze = false, isFrozen = false, freezeUnlocked = true;
    private readonly float freezeCdDuration = 30f, freezeDuration = 15f;
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

        flashUnlocked = PlayerPrefs.GetInt(GameConstants.MLUPGRADEFLASH, 1) > 1;
        freezeUnlocked = PlayerPrefs.GetInt(GameConstants.MLUPGRADEFREEZE, 1) > 1;
    }

    private void Start()
    {
        ResetFlashCD();

        canFreeze = false;
        freezeCdTime = 0f;
    }

    private void Update()
    {
        if (flashUnlocked && canFlash && Input.GetKeyDown(KeyCode.Q))
        {
            ResetFlashCD();
            GameEvents.OnMLFlashPowerStarted.Publish(true);
        }

        if (freezeUnlocked && canFreeze && Input.GetKeyDown(KeyCode.E))
        {
            freezeTime = 0f;
            canFreeze = false;
            isFrozen = true;
            GameEvents.OnMLFreezeStateUpdated.Publish(true);
            print("Freeze " + isFrozen);
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
                    GameEvents.ONMLFlashCooldownUpdated.Publish(flashCdTime / flashCdDuration);
                }
                else
                {
                    canFlash = true;
                }
            }
        }

        if (freezeUnlocked)
        {
            //Frozen active countdown
            if (isFrozen)
            {
                freezeTime += Time.fixedDeltaTime;
                if (freezeTime >= freezeDuration)
                {
                    isFrozen = false;
                    canFreeze = true;
                    freezeTime = 0f;
                    freezeCdTime = 0f;
                    GameEvents.OnMLFreezeStateUpdated.Publish(false);
                    print("Freeze: " + isFrozen);
                }
            }
            else //Frozen Cooldown (when not frozen)
            {
                if (!canFreeze)
                {
                    if (freezeCdTime < freezeCdDuration)
                    {
                        freezeCdTime += Time.fixedDeltaTime;
                        GameEvents.ONMLFreezeCooldownUpdated.Publish(freezeCdTime / freezeCdDuration);
                    }
                    else
                    {
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
    }
}
