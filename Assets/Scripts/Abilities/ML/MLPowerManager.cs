using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPowerManager : MonoBehaviour
{
    public static MLPowerManager Instance { get; private set; }

    private bool canFlash = false;
    private readonly float flashCdDuration = 10f;
    private float flashCdTime = 0f;

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
    }

    private void Update()
    {
        if (canFlash && Input.GetKeyDown(KeyCode.F))
        {
            ResetFlashCD();
            GameEvents.OnMLFlashPowerStarted.Publish(true);
        }
    }

    private void FixedUpdate()
    {
        if (!canFlash)
        {
            if (flashCdTime < flashCdDuration)
            {
                flashCdTime += Time.fixedDeltaTime;
                print(flashCdTime / flashCdDuration);
            }
            else
            {
                canFlash = true;
            }
        }
    }

    private void ResetFlashCD()
    {
        canFlash = false;
        flashCdTime = 0f;
    }
}
