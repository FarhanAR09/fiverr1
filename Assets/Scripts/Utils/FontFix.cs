using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontFix : MonoBehaviour
{
    public Font[] fonts;

    void Start()
    {
        foreach (var font in fonts)
        {
            print($"{font.name} chagned");
            font.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
}
