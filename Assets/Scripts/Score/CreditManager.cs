using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreditManager
{
    public static float Credit {
        get;
        private set; 
    }

    public static void LoadCredit()
    {
        Credit = PlayerPrefs.GetFloat(GameConstants.CREDIT, 0f);
    }

    public static void SaveCredit()
    {
        PlayerPrefs.SetFloat(GameConstants.CREDIT, Credit);
        PlayerPrefs.Save();
    }

    public static void DepositCredit(float amount)
    {
        LoadCredit();
        //Credit = PlayerPrefs.GetFloat(GameConstants.CREDIT, 0f);
        Credit += amount;
        GameEvents.OnCreditUpdated.Publish(Credit);
        //Debug.Log($"Credits Left: {Credit}");
        SaveCredit();
    }

    public static bool TrySpendCredit(float amount)
    {
        LoadCredit();
        //Debug.Log("Spending...");
        //Credit = PlayerPrefs.GetFloat(GameConstants.CREDIT, 0f);
        if (amount <= Credit)
        {
            //Debug.Log("Spending Succeed");
            Credit -= amount;
            SaveCredit();
            GameEvents.OnCreditUpdated.Publish(Credit);
            //Debug.Log($"Credits Left: {Credit}");
            return true;
        }
        SaveCredit();
        //Debug.Log($"Credits Left: {Credit}");
        return false;
    }
}
