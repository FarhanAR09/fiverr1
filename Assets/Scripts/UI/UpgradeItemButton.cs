using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeItemButton : MonoBehaviour
{
    public enum Game
    {
        FTC, ML
    }

    private Button button;
    [SerializeField]
    private UpgradeItem upgradeItem;
    [SerializeField]
    private Image iconDisplay;
    [SerializeField]
    private TMP_Text nameDisplay, levelDisplay;

    [SerializeField]
    private Game game = Game.FTC;
    private string Key
    {
        get
        {
            return game switch
            {
                Game.FTC => GameConstants.FTCCREDIT,
                Game.ML => GameConstants.MLCREDIT,
                _ => GameConstants.FTCCREDIT,
            };
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(TryUpgrade);
        UpdateDisplay(true);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(TryUpgrade);
    }

    private void TryUpgrade()
    {
        if (upgradeItem != null && PlayerPrefs.GetInt(upgradeItem.KeyName) < upgradeItem.MaxLevel)
        {
            int level = PlayerPrefs.GetInt(upgradeItem.KeyName, 1);
            int price = 0;
            if (level - 1 >= 0 && level - 1 < upgradeItem.MaxLevel - 1)
            {
                price = upgradeItem.Price[level - 1];
            }
            else Debug.LogError("Price array mistake");

            CreditManager.LoadCredit(Key);
            if (CreditManager.TrySpendCredit(Key, price))
            {
                upgradeItem.TryUpgrade(callback: UpdateDisplay);
                //Debug.Log("Enough Credit");
            }
            CreditManager.SaveCredit(Key);
        }
        else Debug.LogWarning("Upgrade Failed");
    }

    private void UpdateDisplay(bool success)
    {
        if (success && upgradeItem != null)
        {
            int level = PlayerPrefs.GetInt(upgradeItem.KeyName, 1);
            if (level < 1)
            {
                PlayerPrefs.SetInt(upgradeItem.KeyName, 1);
                level = 1;
            }
            if (iconDisplay != null)
            {
                if (level - 1 < upgradeItem.Icons.Length)
                    iconDisplay.sprite = upgradeItem.Icons[level - 1];
            }
            if (levelDisplay != null)
            {
                int price = 0;
                if (level - 1 >= 0 && level - 1 < upgradeItem.MaxLevel - 1)
                {
                    price = upgradeItem.Price[level - 1];
                }
                if (upgradeItem.MaxLevel > 2)
                {
                    if (level < upgradeItem.MaxLevel)
                        levelDisplay.SetText("Level " + level + "\n" + price);
                    else
                        levelDisplay.SetText("Level MAX");
                }
                else
                {
                    if (level < upgradeItem.MaxLevel)
                        levelDisplay.SetText("LOCKED" + "\n" + price);
                    else
                        levelDisplay.SetText("UNLOCKED");
                }
            }
            if (nameDisplay != null)
            {
                nameDisplay.SetText(upgradeItem.Name);
            }

            //if (level > upgradeItem.MaxLevel)
            //{
            //    PlayerPrefs.SetInt(upgradeItem.KeyName, 1);
            //    //Debug.Log("Resetting...");
            //}
        }
        else
        {
            Debug.LogWarning("Upgrade Failed");
        }
    }
}
