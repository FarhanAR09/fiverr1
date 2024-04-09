using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GatesManager : MonoBehaviour
{
    [SerializeField]
    private MapData GateSpawnableArea;
    [SerializeField]
    private GameObject gatePrefab;
    [SerializeField]
    private GateDisplay gateDisplay1, gateDisplay2, gateDisplay3;

    private readonly int maxGateNumber = 3;
    private int spawnedGateNumber = 0;
    private int collectedGateNumber = 0;

    private List<Vector2Int> spawnedGatePositions = new();

    public UnityEvent OnAllGatesCollected { get; private set; } = new();

    private List<GameObject> spawnedGates = new();

    [SerializeField]
    private AudioClip speedupSFX;

    private void OnDestroy()
    {
        StopAllCoroutines();
        OnAllGatesCollected.RemoveAllListeners();
    }

    public void SpawnGates()
    {
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            if (GateSpawnableArea != null)
            {
                if (gatePrefab != null)
                {
                    //Destroy remaining pellets
                    foreach (GameObject pellet in spawnedGates)
                        if (pellet != null)
                            Destroy(pellet);
                    spawnedGates.Clear();

                    gateDisplay1.ChangeState(false);
                    gateDisplay2.ChangeState(false);
                    gateDisplay3.ChangeState(false);

                    collectedGateNumber = 0;
                    spawnedGateNumber = 0;
                    spawnedGatePositions.Clear();
                    for (int count = 0; count < maxGateNumber; count++)
                    {
                        Vector2Int pickedPosition = TryGetRandomTilePosition();
                        if (pickedPosition != new Vector2Int(-1, -1))
                        {
                            spawnedGateNumber++;
                            float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                            Vector2 worldPosition = (Vector2) MapHandler.Instance.MapGrid.GetWorldPosition(pickedPosition.x, pickedPosition.y) + new Vector2(cellSize/2, cellSize/2);
                            GatePellet gate = Instantiate(gatePrefab, worldPosition, new Quaternion()).GetComponent<GatePellet>();
                            gate.OnCollected.AddListener(CountCollected);
                            spawnedGates.Add(gate.gameObject);
                        }
                        else //Failed to get position
                        {
                            Debug.LogWarning("Failed to spawn a gate. Gate not spawned");
                        }
                    }
                }
                else Debug.Log("Gate Prefab is null");
            }
            else Debug.LogWarning("Gate Spawnable Area is null");
        }
    }

    private void CountCollected()
    {
        collectedGateNumber++;
        if (collectedGateNumber >= spawnedGateNumber)
        {
            IEnumerator DelayNextLevel()
            {
                yield return new WaitForSeconds(1f);
                OnAllGatesCollected.Invoke();
                if (speedupSFX != null && SFXController.Instance != null)
                    SFXController.Instance.RequestPlay(speedupSFX, 20000);

            }
            StopCoroutine(DelayNextLevel());
            StartCoroutine(DelayNextLevel());
        }

        switch (collectedGateNumber)
        {
            case 1:
                gateDisplay1.ChangeState(true);
                break;
            case 2:
                gateDisplay2.ChangeState(true);
                break;
            case 3:
                gateDisplay3.ChangeState(true);
                break;
        }
    }

    private Vector2Int TryGetRandomTilePosition()
    {
        int maxTries = 512;
        if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
        {
            int maxWidth = MapHandler.Instance.MapGrid.GetWidth();
            int maxHeight = MapHandler.Instance.MapGrid.GetHeight();
            for (int i = 0; i < maxTries; i++)
            {
                Vector2Int randomTilePos = new(UnityEngine.Random.Range(0, maxWidth), UnityEngine.Random.Range(0, maxHeight));
                if (GateSpawnableArea.GetData(randomTilePos.x, randomTilePos.y) && !spawnedGatePositions.Exists(spawnedGatePosition => Mathf.Abs((randomTilePos - spawnedGatePosition).x) < 3 && Mathf.Abs((randomTilePos - spawnedGatePosition).y) < 3))
                {
                    spawnedGatePositions.Add(randomTilePos);
                    return randomTilePos;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
}
