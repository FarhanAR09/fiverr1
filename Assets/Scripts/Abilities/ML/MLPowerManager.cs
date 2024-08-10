using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPowerManager : MonoBehaviour
{
    public static MLPowerManager Instance { get; private set; }

    private bool canFlash = false;
    private readonly float flashCdDuration = 30f;
    private float flashCdTime = 0f;

    private bool canFreeze = false, isFrozen = false;
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
    }

    private void Start()
    {
        ResetFlashCD();

        canFreeze = false;
        freezeCdTime = 0f;
    }

    private void Update()
    {
        if (canFlash && Input.GetKeyDown(KeyCode.Q))
        {
            ResetFlashCD();
            GameEvents.OnMLFlashPowerStarted.Publish(true);
        }

        if (canFreeze && Input.GetKeyDown(KeyCode.E))
        {
            freezeTime = 0f;
            canFreeze = false;
            isFrozen = true;
            GameEvents.OnMLFreezePowerUpdated.Publish(true);
            print("Freeze " + isFrozen);
        }
    }

    private void FixedUpdate()
    {
        if (!canFlash)
        {
            if (flashCdTime < flashCdDuration)
            {
                flashCdTime += Time.fixedDeltaTime;
            }
            else
            {
                canFlash = true;
            }
        }

        //Frozen Cooldown (when not frozen)
        if (!canFreeze && !isFrozen)
        {
            if (freezeCdTime < freezeCdDuration)
            {
                freezeCdTime += Time.fixedDeltaTime;
            }
            else
            {
                canFreeze = true;
            }
        }

        //Frozen countdown
        if (isFrozen)
        {
            freezeTime += Time.fixedDeltaTime;
            if (freezeTime >= freezeDuration)
            {
                isFrozen = false;
                canFreeze = true;
                freezeTime = 0f;
                freezeCdTime = 0f;
                GameEvents.OnMLFreezePowerUpdated.Publish(false);
                print("Freeze " + isFrozen);
            }
        }
    }

    private void ResetFlashCD()
    {
        canFlash = false;
        flashCdTime = 0f;
    }
}
