using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles level start and level up (includes triggering spawn)
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GatesManager gatesManager;
    [SerializeField]
    private ScoreHandler scoreHandler;

    [SerializeField]
    private float levelUpSpeedUp = 0.1f;
    private const string LEVELSPEEDKEY = "LevelTimeSpeedUp";
    private float CurrentSpeed
    {
        get => 1 + level * levelUpSpeedUp;
    }

    private int level = 0;

    private void OnEnable()
    {
        if (gatesManager != null)
        {
            gatesManager.OnAllGatesCollected.AddListener(LevelUp);
        }
    }

    private void Start()
    {
        if (gatesManager != null)
            gatesManager.SpawnGates();
        if (scoreHandler != null)
            scoreHandler.SpawnPellets();

        GameSpeedManager.TryAddGameSpeedModifier(LEVELSPEEDKEY, CurrentSpeed);
    }

    private void FixedUpdate()
    {
        //Debug.Log("GameSpeed: " + Time.timeScale);
    }

    private void OnDisable()
    {
        if (gatesManager != null)
        {
            gatesManager.OnAllGatesCollected.RemoveListener(LevelUp);
        }
    }

    private void OnDestroy()
    {
        GameSpeedManager.RemoveGameSpeedModifier(LEVELSPEEDKEY);
    }

    private void LevelUp()
    {
        GameEvents.OnLevelUp.Publish(true);

        level++;

        if (gatesManager != null)
        {
            gatesManager.SpawnGates();
        }
        if (scoreHandler != null)
        {
            scoreHandler.SpawnPellets();
        }

        if (!GameSpeedManager.TryModifyGameSpeedModifier(LEVELSPEEDKEY, CurrentSpeed))
            GameSpeedManager.TryAddGameSpeedModifier(LEVELSPEEDKEY, CurrentSpeed);
    }
}
