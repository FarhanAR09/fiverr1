using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreditManager
{
    /// <summary>
    /// Dictionary of various types of credits. Key is same for PlayerPrefs
    /// </summary>
    public static Dictionary<string,float> Credit {
        get;
        private set; 
    }

    public static void LoadCredit(string key)
    {
        if (!Credit.ContainsKey(key)) Credit.Add(key, 0f);
        Credit[key] = PlayerPrefs.GetFloat(key, 0f);
    }

    public static void SaveCredit(string key)
    {
        if (!Credit.ContainsKey(key)) Credit.Add(key, 0f);
        PlayerPrefs.SetFloat(GameConstants.FTCCREDIT, Credit[key]);
        PlayerPrefs.Save();
    }

    public static void DepositCredit(string key, float amount)
    {
        LoadCredit(key);
        //Credit = PlayerPrefs.GetFloat(GameConstants.CREDIT, 0f);
        if (!Credit.ContainsKey(key)) Credit.Add(key, 0f);
        Credit[key] += amount;
        GameEvents.OnCreditUpdated.Publish(Credit[key]);
        //Debug.Log($"Credits Left: {Credit}");
        SaveCredit(key);
    }

    public static bool TrySpendCredit(string key, float amount)
    {
        LoadCredit(key);
        //Debug.Log("Spending...");
        //Credit = PlayerPrefs.GetFloat(GameConstants.CREDIT, 0f);
        if (amount <= Credit[key])
        {
            //Debug.Log("Spending Succeed");
            Credit[key] -= amount;
            SaveCredit(key);
            GameEvents.OnCreditUpdated.Publish(Credit[key]);
            //Debug.Log($"Credits Left: {Credit}");
            return true;
        }
        SaveCredit(key);
        //Debug.Log($"Credits Left: {Credit}");
        return false;
    }

    public static float GetCredit(string key)
    {
        if (!Credit.ContainsKey(key))
        {
            Credit.Add(key, 0f);
            return 0f;
        }
        return Credit[key];
    }
}
