using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField]
    private MapData ScoreMapData;
    [SerializeField]
    private GameObject pelletPrefab;

    private List<GameObject> spawnedPellets = new();

    private int width, height;

    private void Awake()
    {
        width = ScoreMapData.GetWidth();
        height = ScoreMapData.GetHeight();
    }

    public void SpawnPellets()
    {
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            if (ScoreMapData != null && pelletPrefab != null)
            {
                if (MapHandler.Instance.MapGrid.GetWidth() == ScoreMapData.GetWidth() && MapHandler.Instance.MapGrid.GetHeight() == ScoreMapData.GetHeight())
                {
                    //Destroy remaining pellets
                    foreach (GameObject pellet in spawnedPellets) if (pellet != null) Destroy(pellet);
                    spawnedPellets.Clear();

                    for (int col = 0; col < width; col++)
                    {
                        for (int row = 0; row < height; row++)
                        {
                            //Current tile
                            if (ScoreMapData.GetData(col, row))
                            {
                                Vector3 spawnPos = MapHandler.Instance.MapGrid.GetWorldPosition(col, row) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0);
                                spawnedPellets.Add(Instantiate(pelletPrefab, spawnPos, new Quaternion()));

                                //Tile above
                                if (CheckBoundary(row + 1, height) && ScoreMapData.GetData(col, row + 1))
                                {
                                    spawnPos = Vector3.Lerp(
                                        MapHandler.Instance.MapGrid.GetWorldPosition(col, row) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0),
                                        MapHandler.Instance.MapGrid.GetWorldPosition(col, row + 1) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0),
                                        0.5f
                                        );
                                    spawnedPellets.Add(Instantiate(pelletPrefab, spawnPos, new Quaternion()));
                                }

                                //Tile right
                                if (CheckBoundary(col + 1, width) && ScoreMapData.GetData(col + 1, row))
                                {
                                    spawnPos = Vector3.Lerp(
                                        MapHandler.Instance.MapGrid.GetWorldPosition(col, row) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0),
                                        MapHandler.Instance.MapGrid.GetWorldPosition(col + 1, row) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0),
                                        0.5f
                                        );
                                    spawnedPellets.Add(Instantiate(pelletPrefab, spawnPos, new Quaternion()));
                                }
                            }
                        }
                    }
                }
                else Debug.LogWarning("MapGrid and ScoreMapData have different sizes");
            }
            else Debug.LogWarning("ScoreMapData is null");
        }
    }

    private bool CheckBoundary(int index, int size)
    {
        return index >= 0 && index < size;
    }
}
