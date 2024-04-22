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

    private int orderToCollect = 0;

    private readonly List<Vector2Int> spawnedGatePositions = new();

    private readonly List<GameObject> spawnedGates = new();

    [SerializeField]
    private AudioClip speedupSFX;

    private void OnEnable()
    {
        GameEvents.OnLevelUp.Add(PrepareLevelUp);
    }

    private void OnDisable()
    {
        GameEvents.OnLevelUp.Remove(PrepareLevelUp);
    }

    private void Start()
    {
        //Spawn Gate
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

                    orderToCollect = 0;

                    GatePellet gate = Instantiate(gatePrefab, new Vector2(-99999f, -99999f), new Quaternion()).GetComponent<GatePellet>();
                    gate.Setup(0, gateDisplay1);
                    gate.OnCollected.AddListener(CheckCollected);
                    spawnedGates.Add(gate.gameObject);

                    gate = Instantiate(gatePrefab, new Vector2(-99999f, -99999f), new Quaternion()).GetComponent<GatePellet>();
                    gate.Setup(1, gateDisplay2);
                    gate.OnCollected.AddListener(CheckCollected);
                    spawnedGates.Add(gate.gameObject);

                    gate = Instantiate(gatePrefab, new Vector2(-99999f, -99999f), new Quaternion()).GetComponent<GatePellet>();
                    gate.Setup(2, gateDisplay3);
                    gate.OnCollected.AddListener(CheckCollected);
                    spawnedGates.Add(gate.gameObject);

                    MovePelletsRandomly();
                    GameEvents.OnGatesSequenceUpdate.Publish(orderToCollect);
                }
                else Debug.LogWarning("Gate Prefab is null");
            }
            else Debug.LogWarning("Gate Spawnable Area is null");
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void CheckCollected(int order)
    {
        if (order == orderToCollect)
        {
            orderToCollect++;
            switch (order)
            {
                case 0:
                    gateDisplay1.ChangeState(true);
                    break;
                case 1:
                    gateDisplay2.ChangeState(true);
                    break;
                case 2:
                    gateDisplay3.ChangeState(true);
                    break;
            }

            GameEvents.OnGatesSequenceUpdate.Publish(orderToCollect);

            //All gates collected
            if (orderToCollect >= spawnedGates.Count)
            {
                GameEvents.OnAllGatesCollected.Publish(true);
            }
        }
        else
        {
            orderToCollect = 0;
            GameEvents.OnGatesSequenceUpdate.Publish(0);
            GameEvents.OnGatesWrongSequence.Publish(true);

            gateDisplay1.ChangeState(false);
            gateDisplay2.ChangeState(false);
            gateDisplay3.ChangeState(false);
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

    private void MovePelletsRandomly()
    {
        spawnedGatePositions.Clear();
        foreach (GameObject gate in spawnedGates)
        {
            if (gate != null)
            {
                Vector2Int pickedPosition = TryGetRandomTilePosition();
                if (pickedPosition != new Vector2Int(-1, -1))
                {
                    //spawnedGateNumber++;
                    float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                    Vector2 worldPosition = (Vector2)MapHandler.Instance.MapGrid.GetWorldPosition(pickedPosition.x, pickedPosition.y) + new Vector2(cellSize / 2, cellSize / 2);
                    gate.transform.position = worldPosition;
                }
                else //Failed to get position
                {
                    Debug.LogWarning("Failed to spawn a gate. Gate not moved");
                }
            }
            else Debug.LogWarning("Gate pellet is null");
        }
    }

    private void PrepareLevelUp(bool _)
    {
        orderToCollect = 0;
        gateDisplay1.ChangeState(false);
        gateDisplay2.ChangeState(false);
        gateDisplay3.ChangeState(false);
        MovePelletsRandomly();

        if (speedupSFX != null && SFXController.Instance != null)
            SFXController.Instance.RequestPlay(speedupSFX, 20000);
        GameEvents.OnGatesSequenceUpdate.Publish(0);
    }
}
