using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    private bool isPurging, playerLost = false;

    private float shootTime = 0;
    private readonly float shootDelayReal = 1.2f;

    private readonly int shootMaxAmount = 5;
    private int shootAmount = 0;

    [Tooltip("Where to shoot the lasers")]
    [SerializeField]
    private List<int> rowPositions = new(), columnPositions = new();

    [SerializeField]
    private GameObject laserBeamPrefab;

    private float ShootDelay
    {
        get => shootDelayReal * Time.timeScale;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        GameEvents.OnPurgeStarted.Add(StartShooting);
        GameEvents.OnPurgeFinished.Add(StopShooting);
        GameEvents.OnPlayerLose.Add(HandleLosing);
    }

    private void OnDisable()
    {
        GameEvents.OnPurgeStarted.Remove(StartShooting);
        GameEvents.OnPurgeFinished.Remove(StopShooting);
        GameEvents.OnPlayerLose.Remove(HandleLosing);
    }

    private void FixedUpdate()
    {
        //Shooting
        if (isPurging && !playerLost)
        {
            if (shootTime < ShootDelay)
            {
                shootTime += Time.fixedDeltaTime;
            }
            else if (shootAmount < shootMaxAmount)
            {
                shootTime = 0;

                if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
                {
                    bool isHorizontal = Random.Range(0, 2) != 0;
                    int gridTargetPosition = -1;

                    if (isHorizontal && rowPositions.Count > 0)
                        gridTargetPosition = rowPositions[Random.Range(0, rowPositions.Count)];
                    else if (columnPositions.Count > 0)
                        gridTargetPosition = columnPositions[Random.Range(0, columnPositions.Count)];

                    Vector2 worldTargetPosition;
                    if (isHorizontal)
                    {
                        float middle = MapHandler.Instance.MapGrid.GetWidth() * MapHandler.Instance.MapGrid.GetCellSize() / 2;
                        float yPos = MapHandler.Instance.MapGrid.GetWorldPosition(0, gridTargetPosition).y + MapHandler.Instance.MapGrid.GetCellSize() / 2;
                        worldTargetPosition = new Vector2(middle, yPos);
                    }
                    else
                    {
                        float middle = MapHandler.Instance.MapGrid.GetHeight() * MapHandler.Instance.MapGrid.GetCellSize() / 2;
                        float xPos = MapHandler.Instance.MapGrid.GetWorldPosition(gridTargetPosition, 0).x + MapHandler.Instance.MapGrid.GetCellSize() / 2;
                        worldTargetPosition = new Vector2(xPos, middle);
                    }

                    //Debug.Log("Laser shot at " + (isHorizontal ? "row" : "column") + " " + worldTargetPosition);
                    if (laserBeamPrefab != null)
                    {
                        shootAmount++;
                        GameObject laserBeam = Instantiate(laserBeamPrefab, new Vector3(99999, 99999), new Quaternion());
                        if (laserBeam.TryGetComponent(out LaserBeam lb))
                        {
                            lb.StartLaser(isHorizontal, worldTargetPosition);
                        }
                        else Destroy(laserBeam);
                    }
                }
            }
        }
    }

    private void StartShooting(bool _)
    {
        IEnumerator DelayShooting()
        {
            yield return new WaitForSecondsRealtime(3f);
            isPurging = true;
            shootTime = 0;
            shootAmount = 0;
        }
        StopCoroutine(DelayShooting());
        StartCoroutine(DelayShooting());
    }

    private void StopShooting(bool _)
    {
        isPurging = false;
    }

    private void HandleLosing(bool _)
    {
        playerLost = true;
    }
}
