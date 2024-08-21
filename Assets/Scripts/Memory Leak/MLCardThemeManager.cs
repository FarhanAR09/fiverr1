using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class MLCardThemeManager : MonoBehaviour
{
    public static MLCardThemeManager Instance { get; private set; }

    public MLCardThemeAssets ThemeAssets { get; private set; }

    private void Awake()
    {
        //Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Load Assets
        //TODO: Load theme according to input
        int themeIndex = PlayerPrefs.GetInt(GameConstants.MLSELECTCARDTHEMEINDEX, 0);
        string assetsPath = themeIndex switch
        {
            0 => GameConstants.MLCARDTHEMEASSETSPATHTEST1,
            1 => GameConstants.MLCARDTHEMEASSETSPATHTEST2,
            _ => GameConstants.MLCARDTHEMEASSETSPATHTEST1
        };
        ThemeAssets = Resources.Load<MLCardThemeAssets>(assetsPath);
        if (ThemeAssets == null)
        {
            Debug.LogError("Resource was not found");
        }
    }

    public Sprite GetCardIcon(int cardNumber)
    {
        if (ThemeAssets == null)
        {
            Debug.LogError("ThemeAssets is null");
            return null;
        }   
        if (ThemeAssets.icons == null)
        {
            Debug.LogError("Icon list is null");
            return null;
        }

        if (cardNumber < 0)
        {
            Debug.LogError("Card number is negative");
            return null;
        }
        if (!(cardNumber < ThemeAssets.icons.Count))
        {
            Debug.LogError("Asset for this card number is unavailable");
            return null;
        }

        return ThemeAssets.icons[cardNumber];
    }

    public string GetCardName(int cardNumber)
    {
        if (ThemeAssets == null)
        {
            Debug.LogError("ThemeAssets is null");
            return "";
        }
        if (ThemeAssets.names == null)
        {
            Debug.LogError("Name list is null");
            return "";
        }

        if (cardNumber < 0)
        {
            Debug.LogError("Card number is negative");
            return "";
        }
        if (!(cardNumber < ThemeAssets.names.Count))
        {
            Debug.LogError("Asset for this card number is unavailable");
            return "";
        }

        return ThemeAssets.names[cardNumber];
    }
}
