using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePageManager : MonoBehaviour
{
    private static UpgradePageManager _instance;

    public static UpgradePageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new();
                _instance = singletonObject.AddComponent<UpgradePageManager>();
                singletonObject.name = "Upgrade Page Manager";
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
}
