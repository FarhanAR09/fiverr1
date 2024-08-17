using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class MLMainMenuFeatureSwitches : MonoBehaviour
{
    [SerializeField]
    private RectTransform featureSwitchCanvas;
    [SerializeField]
    private Camera renderingCamera;

    [SerializeField]
    private Toggle overrideDifficultyToggle;
    [SerializeField]
    private TMP_InputField gridSizeInputCol, gridSizeInputRow, maxLeakInput;

    public static bool difficultyOverridden = false;
    public static int debugColumnCount = 2, debugRowCount = 2;

    public static int DebugMaxLeak { get; private set; } = 100;

    private void Awake()
    {
        //Canvas size
        if (featureSwitchCanvas != null)
        {
            featureSwitchCanvas.position = new Vector3(renderingCamera.transform.position.x, renderingCamera.transform.position.y, featureSwitchCanvas.transform.position.z);
            float height = 2f * renderingCamera.orthographicSize;
            float width = renderingCamera.aspect * height;
            featureSwitchCanvas.sizeDelta = new Vector2(width, height);

            featureSwitchCanvas.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (featureSwitchCanvas != null)
        {
            featureSwitchCanvas.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (overrideDifficultyToggle != null)
            overrideDifficultyToggle.onValueChanged.AddListener(OverrideDifficulty);
        if (gridSizeInputCol != null)
            gridSizeInputCol.onEndEdit.AddListener(ChangeColumnCount);
        if (gridSizeInputRow != null)
            gridSizeInputRow.onEndEdit.AddListener(ChangeRowCount);
        if (maxLeakInput != null)
            maxLeakInput.onEndEdit.AddListener(ChangeMaxLeak);
    }

    private void OnDisable()
    {
        if (overrideDifficultyToggle != null)
            overrideDifficultyToggle.onValueChanged.RemoveListener(OverrideDifficulty);
        if (gridSizeInputCol != null)
            gridSizeInputCol.onEndEdit.RemoveListener(ChangeColumnCount);
        if (gridSizeInputRow != null)
            gridSizeInputRow.onEndEdit.RemoveListener(ChangeRowCount);
        if (maxLeakInput != null)
            maxLeakInput.onEndEdit.RemoveListener(ChangeMaxLeak);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (featureSwitchCanvas != null)
            {
                if (featureSwitchCanvas.gameObject.activeInHierarchy)
                {
                    featureSwitchCanvas.gameObject.SetActive(false);
                    //if (!GameSpeedManager.TryModifyGameSpeedModifier("FSPause", 1))
                    //    GameSpeedManager.TryAddGameSpeedModifier("FSPause", 1);
                }
                else
                {
                    featureSwitchCanvas.gameObject.SetActive(true);
                    //if (!GameSpeedManager.TryModifyGameSpeedModifier("FSPause", 0))
                    //    GameSpeedManager.TryAddGameSpeedModifier("FSPause", 0);

                    //Update Inputs to Variables
                    if (gridSizeInputCol != null)
                    {
                        gridSizeInputCol.text = debugColumnCount.ToString();
                    }
                    if (gridSizeInputRow != null)
                    {
                        gridSizeInputRow.text = debugRowCount.ToString();
                    }
                    if (maxLeakInput != null)
                    {
                        maxLeakInput.text = DebugMaxLeak.ToString();
                    }
                }
            }
        }
    }

    private void OverrideDifficulty(bool overridden)
    {
        difficultyOverridden = overridden;
    }

    private void ChangeColumnCount(string _)
    {
        if (gridSizeInputCol != null)
        {
            if (int.TryParse(gridSizeInputCol.text, out int amount))
            {
                debugColumnCount = amount;
                if (debugColumnCount * debugRowCount % 2 != 0)
                {
                    debugColumnCount++;
                }
                gridSizeInputCol.text = debugColumnCount.ToString();
            }
            else
            {
                Debug.LogWarning("Invalid Input: should be number");
            }
        }
    }

    private void ChangeRowCount(string _)
    {
        if (gridSizeInputRow != null)
        {
            if (int.TryParse(gridSizeInputRow.text, out int amount))
            {
                debugRowCount = amount;
                if (debugColumnCount * debugRowCount % 2 != 0)
                {
                    debugRowCount++;
                }
                gridSizeInputRow.text = debugRowCount.ToString();
            }
            else
            {
                Debug.LogWarning("Invalid Input: should be number");
            }
        }
    }

    private void ChangeMaxLeak(string _)
    {
        if (maxLeakInput != null)
        {
            if (int.TryParse(maxLeakInput.text, out int amount))
            {
                DebugMaxLeak = Mathf.Max(0, amount);
                maxLeakInput.text = DebugMaxLeak.ToString();
            }
            else
            {
                Debug.LogWarning("Invalid Input: should be number");
            }
        }
    }

    private void PurgeState(bool state)
    {
        GameEvents.OnSwitchPurge.Publish(state);
    }

    private void SpeedUpState(bool state)
    {
        GameEvents.OnSwitchSpeedUp.Publish(state);
    }

    private void SlowDownState(bool state)
    {
        GameEvents.OnSwitchSlowDown.Publish(state);
    }

    private void RequireChargeState(bool state)
    {
        GameEvents.OnSwitchRequireCharge.Publish(state);
    }

    private void CooldownBatteryState(bool state)
    {
        GameEvents.OnSwitchCooldownCharge.Publish(state);
    }

    private void PowersCooldownState(bool state)
    {
        GameEvents.OnSwitchPowersCooldown.Publish(state);
    }

    private void SequentialGatesState(bool active)
    {
        GameEvents.OnSwitchSequentialGates.Publish(active);
    }

    private void IdleMechanicState(bool active)
    {
        GameEvents.OnSwitchIdleMechanics.Publish(active);
    }

    private void SpawnBitsEaters(bool active)
    {
        GameEvents.OnSwitchSpawnBitsEaters.Publish(active);
    }

    private void SpawnTrojanHorses(bool active)
    {
        GameEvents.OnSwitchSpawnTrojanHorses.Publish(active);
    }

    private void SpawnQuantumGhosts(bool active)
    {
        GameEvents.OnSwitchSpawnQuantumGhosts.Publish(active);
    }
}
