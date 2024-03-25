using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
    public Grid<MapTile> MapGrid { get; private set; }

    [SerializeField]
    private MapData MapData, ScoreMapData;

    [SerializeField]
    private GameObject pelletPrefab;

    //Singleton
    public static MapHandler Instance { get; private set; }

    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Grid and Data
        if (MapData != null)
        {
            MapGrid = new Grid<MapTile>(MapData.GetWidth(), MapData.GetHeight(), 1.2f, Vector3.back, (Grid<MapTile> g, int x, int y) => new MapTile());
            if (MapGrid.GetWidth() == MapData.GetWidth() && MapGrid.GetHeight() == MapData.GetHeight())
            {
                for (int row = 0; row < MapData.collisionsColumns.Length; row++)
                {
                    for (int column = 0; column < MapData.collisionsColumns[row].collisionRows.Length; column++)
                    {
                        MapGrid.SetGridObject(row, column, new MapTile(MapData.GetData(row, column)));
                    }
                }
            }
            else Debug.LogWarning("Grid and Data have different sizes");
        }
        else Debug.LogWarning("Map Data is null");

        //Spawn Score
        SpawnPellets();
    }

    public bool CheckBoundary(Vector2Int position)
    {
        if (position.x >= 0 && position.x < MapGrid.GetWidth() && position.y >= 0 && position.y < MapGrid.GetHeight())
            return true;
        else 
            return false;
    }

    private void SpawnPellets()
    {
        if (ScoreMapData != null && pelletPrefab != null)
        {
            if (MapGrid.GetWidth() == ScoreMapData.GetWidth() && MapGrid.GetHeight() == ScoreMapData.GetHeight())
            {
                for (int row = 0; row < MapData.collisionsColumns.Length; row++)
                {
                    for (int column = 0; column < MapData.collisionsColumns[row].collisionRows.Length; column++)
                    {
                        //MapGrid.SetGridObject(row, column, new MapTile(MapData.GetData(row, column)));
                        if (ScoreMapData.GetData(row, column))
                        {
                            Vector3 spawnPos = MapGrid.GetWorldPosition(row, column) + new Vector3(MapGrid.GetCellSize() / 2, MapGrid.GetCellSize() / 2, 0);
                            Instantiate(pelletPrefab, spawnPos, new Quaternion());
                        }
                    }
                }
            }
            else Debug.LogWarning("MapGrid and ScoreMapData have different sizes");
        }
        else Debug.LogWarning("ScoreMapData is null");
    }
}
