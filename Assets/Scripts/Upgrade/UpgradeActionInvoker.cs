using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeActionInvoker : MonoBehaviour
{
    public class InvokeActionParams
    {
        public UpgradeItem upgradeItem;
        public UnityAction<bool> callback;

        public InvokeActionParams(UpgradeItem upgradeItem, UnityAction<bool> callback)
        {
            this.upgradeItem = upgradeItem;
            this.callback = callback;
        }
    }

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

    public void InvokeAction(UpgradeItem upgradeItem, string name = "DefaultUpgradeAction", UnityAction<bool> callback = null)
    {
        //StartCoroutine(name, callback);
        StartCoroutine(upgradeItem.ActionName, new InvokeActionParams(upgradeItem, callback));
    }

    private void BaseUpgrade(InvokeActionParams args)
    {
        //Debug.Log("Upgrading...");
        //Debug.Log(PlayerPrefs.GetInt(key));
        PlayerPrefs.SetInt(args.upgradeItem.KeyName, PlayerPrefs.GetInt(args.upgradeItem.KeyName, 1) + 1);
        PlayerPrefs.Save();
        //Debug.Log(PlayerPrefs.GetInt(key));
        args.callback?.Invoke(true);
    }

    private IEnumerator DefaultUpgradeAction(InvokeActionParams args)
    {
        Debug.LogWarning("Invalid Upgrade Action Name!");
        args.callback?.Invoke(false);
        yield return null;
    }

    private IEnumerator BaseUpgradeAction(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator TestUpgradeAction(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator MaxLivesUpgrade(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator TimeSlowUpgrade(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator EMPUpgrade(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator BoostUpgrade(InvokeActionParams args)
    {
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator MLUpgradeDifficulties(InvokeActionParams args)
    {
        print("MLUpgradeDifficulties");
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator MLUpgradeFlash(InvokeActionParams args)
    {
        print("MLUpgradeFlash");
        BaseUpgrade(args);
        yield return null;
    }

    private IEnumerator MLUpgradeFreeze(InvokeActionParams args)
    {
        print("MLUpgradeFreeze");
        BaseUpgrade(args);
        yield return null;
    }
}