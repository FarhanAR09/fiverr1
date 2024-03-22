using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Data", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    [System.Serializable]
    public class MapDataColumn
    {
        [SerializeField]
        public bool[] collisionRows;
    }

    [SerializeField]
    public MapDataColumn[] collisionsColumns;

    public bool GetData(int row, int column) => collisionsColumns[row].collisionRows[column];
    public int GetWidth() => collisionsColumns.Length;
    public int GetHeight() => collisionsColumns[0].collisionRows.Length;
}
