using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPlayManager : MonoBehaviour
{
    public static MLPlayManager Instance { get; private set; }
    public int CurrentLevel { get; private set; } = 0;

    private void OnEnable()
    {
        GameEvents.OnAllCardsPaired.Add(UnlockNewLevel);
    }

    private void OnDisable()
    {
        GameEvents.OnAllCardsPaired.Remove(UnlockNewLevel);
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
        print("Loaded Level: " + CurrentLevel);
        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.SetupByLevel(CurrentLevel);
        }
    }

    private void UnlockNewLevel(bool _)
    {
        int savedUnlockedLevel = PlayerPrefs.GetInt(GameConstants.MLUNLOCKEDLEVEL);
        if (savedUnlockedLevel < CurrentLevel + 1)
        {
            PlayerPrefs.SetInt(GameConstants.MLUNLOCKEDLEVEL, CurrentLevel + 1);
            PlayerPrefs.Save();
        }
    }
}
