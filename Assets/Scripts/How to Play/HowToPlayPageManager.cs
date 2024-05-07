using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HowToPlayPageManager : MonoBehaviour
{
    [Serializable]
    public struct HowToPlayPageData
    {
        public string title, desc;
        public Sprite imageSprite;
    }

    [SerializeField]
    private List<HowToPlayPageData> pageData;

    [SerializeField]
    private TMPro.TextMeshProUGUI titleDisplay, descDisplay;
    [SerializeField]
    private UnityEngine.UI.Image imageDisplay;

    private int pageIndex = 0;

    private void Start()
    {
        UpdatePage(0);
    }

    public void GoToNextHowTo()
    {
        if (pageIndex < pageData.Count - 1)
        {
            pageIndex++;
            UpdatePage(pageIndex);
        }
    }

    public void GoToPreviousHowTo()
    {
        if (pageIndex > 0)
        {
            pageIndex--;
            UpdatePage(pageIndex);
        }
    }

    private void UpdatePage(int index)
    {
        if (index >= 0 && index < pageData.Count)
        {
            if (titleDisplay != null)
            {
                titleDisplay.SetText(pageData[index].title);
            }
            if (descDisplay != null)
            {
                descDisplay.SetText(pageData[index].desc);
            }
            if (imageDisplay != null)
            {
                imageDisplay.sprite = pageData[index].imageSprite;
            }
        }
    }
}

