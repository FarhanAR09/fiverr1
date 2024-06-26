using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrojanSpawner : MonoBehaviour
{
    [SerializeField]
    private TrojanBehaviour trojanHorsePrefab;
    //private List<TrojanBehaviour> spawnedTrojans = new();

    [SerializeField]
    private List<Vector2Int> spawnPositions = new();

    private void OnEnable()
    {
        //GameEvents.OnLevelUp.Add(ClearRemaining);
        GameEvents.OnLevelUp.Add(TrySpawnTrojanHorses);
    }

    private void OnDisable()
    {
        //GameEvents.OnLevelUp.Remove(ClearRemaining);
        GameEvents.OnLevelUp.Remove(TrySpawnTrojanHorses);
    }

    private void TrySpawnTrojanHorses(bool _)
    {
        //40% Spawn rate
        //Debug.Log("Spawn Attempt " + random);
        if (trojanHorsePrefab != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null && spawnPositions.Count > 0 && UnityEngine.Random.Range(0f, 100f) <= 40f)
        {
            Vector2Int spawnPosGrid = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            Vector2 spawnPosWorld = MapHandler.Instance.MapGrid.GetWorldPosition(spawnPosGrid.x, spawnPosGrid.y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one;
            TrojanBehaviour spawned = Instantiate(trojanHorsePrefab, spawnPosWorld, new Quaternion());
            spawned.Setup(1.5f, spawnPosGrid, MovementDirection.Left);
            //spawnedTrojans.Add(spawned);
            //Debug.Log($"Spawn successful at {spawnPosGrid} at {MapHandler.Instance.MapGrid.GetWorldPosition(spawnPosGrid.x, spawnPosGrid.y)}");
        }
    }

    //private void ClearRemaining(bool _)
    //{
    //    if (spawnedTrojans != null)
    //    {
    //        foreach (var bitsEater in spawnedTrojans)
    //            if (bitsEater != null)
    //                Destroy(bitsEater.gameObject);
    //        spawnedTrojans.Clear();
    //    }
    //}
}
