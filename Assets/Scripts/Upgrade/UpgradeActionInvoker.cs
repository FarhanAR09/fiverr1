using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UpgradeActionInvoker : MonoBehaviour
{
    private static UpgradeActionInvoker _instance;

    public static UpgradeActionInvoker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UpgradeActionInvoker>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<UpgradeActionInvoker>();
                    singletonObject.name = typeof(UpgradeActionInvoker).ToString() + " (Singleton)";
                }
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

    public bool InvokeAction(string name)
    {
        return StartCoroutine(name);
    }

    private static IEnumerator<bool> DefaultUpgradeAction()
    {
        Debug.LogWarning("Invalid Upgrade Action Name!");
        yield return false;
    }
}
