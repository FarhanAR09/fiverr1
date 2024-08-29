using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    [SerializeField]
    private float preferedHeight = 9, preferedWidth = 16;
    private float PreferedRatio { get => preferedWidth / preferedHeight; }

    private float initSize;

    private Camera scaledCamera;

    private void Start()
    {
        TryGetComponent(out scaledCamera);
        if (scaledCamera != null)
        {
            initSize = scaledCamera.orthographicSize;
            scaledCamera.orthographicSize = initSize * PreferedRatio / scaledCamera.aspect;
        }
    }
}
