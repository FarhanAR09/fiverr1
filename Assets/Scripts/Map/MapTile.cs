using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile
{
    [field: SerializeField]
    public bool Walkable { get; set; }

    public MapTile()
    {
        Walkable = false;
    }

    public MapTile(bool state)
    {
        Walkable = state;
    }

    public override string ToString()
    {
        return Walkable.ToString();
    }
}
