using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLDebugLeakBar : MonoBehaviour
{
    [SerializeField]
    private Transform bar;

    private float maxYScale;

    private void OnEnable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated += UpdateBar;
        }
    }

    private void OnDisable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated -= UpdateBar;
        }
    }

    private void Awake()
    {
        if (bar != null)
        {
            maxYScale = bar.localScale.y;
        }
    }

    private void Start()
    {
        UpdateBar(0);
    }

    private void UpdateBar(int _)
    {
        float norm = (float) MLLeakTracker.Instance.LeakedMemory / MLLeakTracker.Instance.MaxMemory;
        if (bar != null)
            bar.localScale = new Vector3(bar.localScale.x, Mathf.Max(0.05f, Mathf.Lerp(0f, maxYScale, norm)), bar.localScale.z);
    }
}
