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

                    for (int col = 0; col < ScoreMapData.GetWidth(); col++)
                    {
                        for (int row = 0; row < ScoreMapData.GetHeight(); row++)
                        {
                            if (ScoreMapData.GetData(col, row))
                            {
                                Vector3 spawnPos = MapHandler.Instance.MapGrid.GetWorldPosition(col, row) + new Vector3(MapHandler.Instance.MapGrid.GetCellSize() / 2, MapHandler.Instance.MapGrid.GetCellSize() / 2, 0);
                                spawnedPellets.Add(Instantiate(pelletPrefab, spawnPos, new Quaternion()));
                            }
                        }
                    }
                }
                else Debug.LogWarning("MapGrid and ScoreMapData have different sizes");
            }
            else Debug.LogWarning("ScoreMapData is null");
        }
    }
}
