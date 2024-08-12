using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLPlayManager : MonoBehaviour
{
    public static MLPlayManager Instance { get; private set; }

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
        int desiredLevel = PlayerPrefs.GetInt(GameConstants.MLLOADLEVEL, 1);
        print("Loaded Level: " + desiredLevel);
        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.Setup(desiredLevel);
        }
    }
}
