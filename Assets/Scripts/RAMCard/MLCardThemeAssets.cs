using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Theme Assets", menuName = "Data/MLCardThemeAssets", order = 1)]
public class MLCardThemeAssets : ScriptableObject
{
    [Tooltip("Sprites with 1 unit size")]
    public List<Sprite> icons = new();
    public List<string> names = new();
}
