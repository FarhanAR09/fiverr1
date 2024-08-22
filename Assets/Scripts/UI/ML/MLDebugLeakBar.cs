using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLDebugLeakBar : MonoBehaviour
{
    [SerializeField]
    private Transform cover;

    [SerializeField]
    private float height;

    private void OnEnable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated += UpdateBar;
        }
        else Debug.LogWarning("Tracker null");
    }

    private void OnDisable()
    {
        if (MLLeakTracker.Instance != null)
        {
            MLLeakTracker.Instance.OnMemoryLeakUpdated -= UpdateBar;
        }
        else Debug.LogWarning("Tracker null");
    }

    private void Start()
    {
        UpdateBar(0);
    }

    private void UpdateBar(int _)
    {
        float norm = (float) MLLeakTracker.Instance.LeakedMemory / MLLeakTracker.Instance.MaxMemory;
        if (cover != null)
        {
            cover.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, height / 4f, 0), norm);
            cover.localScale = new Vector3(1, 1 - norm, 1);
        }
    }
}
