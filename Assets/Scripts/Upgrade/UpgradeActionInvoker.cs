using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeActionInvoker : MonoBehaviour
{
    private static UpgradeActionInvoker _instance;

    public static UpgradeActionInvoker Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject();
                _instance = singletonObject.AddComponent<UpgradeActionInvoker>();
                singletonObject.name = typeof(UpgradeActionInvoker).ToString() + " (Singleton)";
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
        }
    }

    public void InvokeAction(string name = "DefaultUpgradeAction", UnityAction<bool> callback = null)
    {
        StartCoroutine(name, callback);
    }

    private void BaseUpgradeAction(string key, UnityAction<bool> callback = null)
    {
        //Debug.Log("Upgrading...");
        //Debug.Log(PlayerPrefs.GetInt(key));
        PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key) + 1);
        PlayerPrefs.Save();
        //Debug.Log(PlayerPrefs.GetInt(key));
        callback?.Invoke(true);
    }

    private IEnumerator DefaultUpgradeAction(UnityAction<bool> callback = null)
    {
        Debug.LogWarning("Invalid Upgrade Action Name!");
        callback?.Invoke(false);
        yield return null;
    }

    private IEnumerator TestUpgradeAction(UnityAction<bool> callback = null)
    {
        BaseUpgradeAction("upgradeTest", callback);
        yield return null;
    }

    private IEnumerator MaxLivesUpgrade(UnityAction<bool> callback = null)
    {
        BaseUpgradeAction("upgradeMaxLives", callback);
        yield return null;
    }

    private IEnumerator TimeSlowUpgrade(UnityAction<bool> callback = null)
    {
        BaseUpgradeAction("upgradeTimeSlow", callback);
        yield return null;
    }

    private IEnumerator EMPUpgrade(UnityAction<bool> callback = null)
    {
        BaseUpgradeAction("upgradeEMP", callback);
        yield return null;
    }

    private IEnumerator BoostUpgrade(UnityAction<bool> callback = null)
    {
        BaseUpgradeAction("upgradeBoost", callback);
        yield return null;
    }

    private IEnumerator MLUpgradeDifficulties(UnityAction<bool> callback = null)
    {
        print("MLUpgradeDifficulties");
        //BaseUpgradeAction("upgradeBoost", callback);
        yield return null;
    }

    private IEnumerator MLUpgradeFlash(UnityAction<bool> callback = null)
    {
        print("MLUpgradeFlash");
        //BaseUpgradeAction("upgradeBoost", callback);
        yield return null;
    }

    private IEnumerator MLUpgradeFreeze(UnityAction<bool> callback = null)
    {
        print("MLUpgradeFreeze");
        //BaseUpgradeAction("upgradeBoost", callback);
        yield return null;
    }
}
