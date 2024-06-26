using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BitsEaterSpawner : MonoBehaviour
{
    [SerializeField]
    private BitsEaterBehaviour bitsEaterPrefab;
    private List<BitsEaterBehaviour> bitsEaters = new();

    [SerializeField]
    private List<Vector2Int> spawnPositions = new();

    private void OnEnable()
    {
        GameEvents.OnLevelUp.Add(ClearBitsEaters);
        GameEvents.OnLevelUp.Add(TrySpawnBitsEaters);
    }

    private void OnDisable()
    {
        GameEvents.OnLevelUp.Remove(ClearBitsEaters);
        GameEvents.OnLevelUp.Remove(TrySpawnBitsEaters);
    }

    private void TrySpawnBitsEaters(bool _)
    {
        //20% Spawn rate
        //Debug.Log("Spawn Attempt " + random);
        if (bitsEaterPrefab != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null && spawnPositions.Count > 0 && UnityEngine.Random.Range(0f, 100f) <= 40f)
        {
            Vector2Int spawnPosGrid = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            Vector2 spawnPosWorld = MapHandler.Instance.MapGrid.GetWorldPosition(spawnPosGrid.x, spawnPosGrid.y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one;
            BitsEaterBehaviour spawned = Instantiate(bitsEaterPrefab, spawnPosWorld, new Quaternion());
            spawned.Setup(1.5f, spawnPosGrid, MovementDirection.Left);
            bitsEaters.Add(spawned);
            //Debug.Log($"Spawn successful at {spawnPosGrid} at {MapHandler.Instance.MapGrid.GetWorldPosition(spawnPosGrid.x, spawnPosGrid.y)}");
        }
    }

    private void ClearBitsEaters(bool _)
    {
        if (bitsEaters != null)
        {
            foreach (var bitsEater in bitsEaters)
                if (bitsEater != null)
                    Destroy(bitsEater.gameObject);
            bitsEaters.Clear();
        }
    }
}
