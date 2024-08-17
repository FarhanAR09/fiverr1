using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class MLPlayManager : MonoBehaviour
{
    public static MLPlayManager Instance { get; private set; }
    public int Difficulty { get; private set; } = 1;
    public MLGameMode GameMode { get; private set; } = MLGameMode.Classic;

    public float CardAutoFlipDuration {
        get {
            return Difficulty switch
            {
                1 => 2.0f,
                2 => 1.6f,
                3 => 1.3f,
                4 => 1.0f,
                5 => 0.7f,
                _ => 1.7f,
            };
        } 
    }

    private void OnEnable()
    {
        if (GameMode == MLGameMode.Endless || GameMode == MLGameMode.Trial)
        {
            GameEvents.OnMLAllCardsPaired.Add(ResetGridTrialEndless);
        }
    }

    private void OnDisable()
    {
        if (GameMode == MLGameMode.Endless || GameMode == MLGameMode.Trial)
        {
            GameEvents.OnMLAllCardsPaired.Remove(ResetGridTrialEndless);
        }
    }

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

        GameMode = (MLGameMode)PlayerPrefs.GetInt(GameConstants.MLLOADGAMEMODE, 0);
        Difficulty = PlayerPrefs.GetInt(GameConstants.MLLOADLEVEL, 1);
    }

    private void Start()
    {
        //print("Loaded Level: " + CurrentLevel);
        print("GameMode: " + GameMode);
        print("Difficulty: " + Difficulty);
        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.SetupByLevel(Difficulty);
        }
        GameEvents.OnMLGameSetup.Publish(true);
    }

    private void ResetGridTrialEndless(bool _)
    {
        if (!(GameMode == MLGameMode.Endless || GameMode == MLGameMode.Trial) || RAMGrid.Instance == null)
            return;
        print("Resetting");
        RAMGrid.Instance.SetupByLevel(Difficulty);
    }
}
