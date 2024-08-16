using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPlayManager : MonoBehaviour
{
    public static MLPlayManager Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    public float CardAutoFlipDuration {
        get {
            return CurrentLevel switch
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

        CurrentLevel = PlayerPrefs.GetInt(GameConstants.MLLOADLEVEL, 1);
    }

    private void Start()
    {
        //print("Loaded Level: " + CurrentLevel);
        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.SetupClassicByLevel(CurrentLevel);
        }
    }
}
