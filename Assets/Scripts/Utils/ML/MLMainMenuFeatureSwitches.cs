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
    private TMP_InputField gridSizeInputCol, gridSizeInputRow, maxLeakInput, maxTrialMistakesInput;

    public static bool DifficultyOverridden { get; private set; } = false;
    public static int DebugColumnCount { get; private set; } = 2;
    public static int DebugRowCount { get; private set; } = 2;

    public static int DebugMaxLeak { get; private set; } = 100;
    public static int DebugTrialMaxMistakes { get; private set; } = 16;

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
        if (maxTrialMistakesInput != null)
            maxTrialMistakesInput.onEndEdit.AddListener(ChangeMaxTrialMistakes);
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
        if (maxTrialMistakesInput != null)
            maxTrialMistakesInput.onEndEdit.RemoveListener(ChangeMaxTrialMistakes);
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

                    //Update Toggles
                    if (overrideDifficultyToggle != null)
                    {
                        overrideDifficultyToggle.isOn = DifficultyOverridden;
                    }

                    //Update Inputs to Variables
                    if (gridSizeInputCol != null)
                    {
                        gridSizeInputCol.text = DebugColumnCount.ToString();
                    }
                    if (gridSizeInputRow != null)
                    {
                        gridSizeInputRow.text = DebugRowCount.ToString();
                    }
                    if (maxLeakInput != null)
                    {
                        maxLeakInput.text = DebugMaxLeak.ToString();
                    }
                    if (maxTrialMistakesInput != null)
                    {
                        maxTrialMistakesInput.text = DebugTrialMaxMistakes.ToString();
                    }
                }
            }
        }
    }

    private void OverrideDifficulty(bool overridden)
    {
        DifficultyOverridden = overridden;
        if (overrideDifficultyToggle != null)
        {
            overrideDifficultyToggle.isOn = DifficultyOverridden;
        }
    }

    private void ChangeColumnCount(string _)
    {
        if (gridSizeInputCol != null)
        {
            if (int.TryParse(gridSizeInputCol.text, out int amount))
            {
                DebugColumnCount = amount;
                if (DebugColumnCount * DebugRowCount % 2 != 0)
                {
                    DebugColumnCount++;
                }
                gridSizeInputCol.text = DebugColumnCount.ToString();
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
                DebugRowCount = amount;
                if (DebugColumnCount * DebugRowCount % 2 != 0)
                {
                    DebugRowCount++;
                }
                gridSizeInputRow.text = DebugRowCount.ToString();
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

    private void ChangeMaxTrialMistakes(string _)
    {
        if (maxTrialMistakesInput != null)
        {
            if (int.TryParse(maxTrialMistakesInput.text, out int amount))
            {
                DebugTrialMaxMistakes = Mathf.Max(0, amount);
                maxTrialMistakesInput.text = DebugTrialMaxMistakes.ToString();
            }
            else
            {
                Debug.LogWarning("Invalid Input: should be number");
            }
        }
    }
}
