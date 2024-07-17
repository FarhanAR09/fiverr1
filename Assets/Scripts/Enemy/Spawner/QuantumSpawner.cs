using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuantumSpawner : MonoBehaviour
{
    [SerializeField]
    private QuantumGhostBehaviour quantumGhostPrefab;
    //private List<TrojanBehaviour> spawnedTrojans = new();

    [SerializeField]
    private List<Vector2Int> spawnPositions = new();

    private bool fsEnabled = true, requirementPowerUpUnlocked = true;

    private void Awake()
    {
        requirementPowerUpUnlocked = PlayerPrefs.GetInt("upgradeEMP", 1) >= 2 || PlayerPrefs.GetInt("upgradeBoost", 1) >= 2;
    }

    private void OnEnable()
    {
        //GameEvents.OnLevelUp.Add(ClearRemaining);
        GameEvents.OnLevelUp.Add(TrySpawnEnemy);
        GameEvents.OnSwitchSpawnQuantumGhosts.Add(FSSetState);
    }

    private void OnDisable()
    {
        //GameEvents.OnLevelUp.Remove(ClearRemaining);
        GameEvents.OnLevelUp.Remove(TrySpawnEnemy);
        GameEvents.OnSwitchSpawnQuantumGhosts.Remove(FSSetState);
    }

    private void TrySpawnEnemy(bool _)
    {
        //40% Spawn rate
        //Debug.Log("Spawn Attempt " + random);
        if (fsEnabled && requirementPowerUpUnlocked && quantumGhostPrefab != null && MapHandler.Instance != null && MapHandler.Instance.MapGrid != null && spawnPositions.Count > 0 && UnityEngine.Random.Range(0f, 100f) <= 40f)
        {
            Debug.Log(name);
            Vector2Int spawnPosGrid = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            Vector2 spawnPosWorld = MapHandler.Instance.MapGrid.GetWorldPosition(spawnPosGrid.x, spawnPosGrid.y) + MapHandler.Instance.MapGrid.GetCellSize() / 2 * Vector3.one;
            QuantumGhostBehaviour spawned = Instantiate(quantumGhostPrefab, spawnPosWorld, new Quaternion());
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

    private void FSSetState(bool enabled)
    {
        fsEnabled = enabled;
    }
}
