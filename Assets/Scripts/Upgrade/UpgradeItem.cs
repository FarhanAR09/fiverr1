using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New UpgradeItem", menuName = "Data/Upgrades", order = 1)]
public class UpgradeItem : ScriptableObject
{
    public string Name;
    public int MaxLevel;
    public string ActionName;
    public string KeyName;
    public int[] Price;
    public Sprite[] Icons;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>level. 0 if failed</returns>
    public int RetrieveLevel()
    {
        return PlayerPrefs.GetInt(KeyName);
    }

    public bool TryUpgrade()
    {
        //TODO: calls action
        return false;
    }
}
