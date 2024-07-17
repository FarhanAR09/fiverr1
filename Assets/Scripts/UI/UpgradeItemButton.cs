using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeItemButton : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private UpgradeItem upgradeItem;
    [SerializeField]
    private Image iconDisplay;
    [SerializeField]
    private TMP_Text nameDisplay, levelDisplay;

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
        //TODO: credit check
        if (upgradeItem != null && PlayerPrefs.GetInt(upgradeItem.KeyName) < upgradeItem.MaxLevel)
        {
            //TODO: reduce credits
            int level = PlayerPrefs.GetInt(upgradeItem.KeyName);
            int price = 0;
            if (level - 1 >= 0 && level - 1 < upgradeItem.MaxLevel - 1)
            {
                price = upgradeItem.Price[level - 1];
            }

            CreditManager.LoadCredit();
            if (CreditManager.TrySpendCredit(price))
            {
                upgradeItem.TryUpgrade(callback: UpdateDisplay);
                Debug.Log("Enough Credit");
            }
            CreditManager.SaveCredit();
        }
        else Debug.LogWarning("Upgrade Failed");
    }

    private void UpdateDisplay(bool success)
    {
        if (success && upgradeItem != null)
        {
            int level = PlayerPrefs.GetInt(upgradeItem.KeyName);
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
            //Debug.Log(upgradeItem.name + " Success");

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
