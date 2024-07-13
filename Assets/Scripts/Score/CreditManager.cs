using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CreditManager
{
    public static int Credit { get; private set; } = 0;

    public static void SaveCredit()
    {
        PlayerPrefs.SetInt(GameConstants.CREDIT, Credit);
    }

    public static void DepositCredit(int amount)
    {
        Credit += amount;
        GameEvents.OnCreditUpdated.Publish(Credit);
    }

    public static void SpendCredit(int amount)
    {
        Credit -= amount;
        GameEvents.OnCreditUpdated.Publish(Credit);
    }
}
