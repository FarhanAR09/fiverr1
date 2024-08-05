using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class WorldCanvasFitter : MonoBehaviour
{
    [Tooltip("If null, this will be main camera")]
    [SerializeField]
    private Camera followedCamera = null;
    [SerializeField]
    private bool followCameraEveryFrame;

    private Canvas canvas;
    private RectTransform canvasTransform;

    private void Awake()
    {
        if (followedCamera == null)
        {
            followedCamera = Camera.main;
        }

        TryGetComponent(out canvas);
        if (canvas != null)
        {
            canvas.TryGetComponent(out canvasTransform);
        }
    }

    private void Start()
    {
        FitCanvas();
    }

    private void Update()
    {
        if (followCameraEveryFrame)
        {
            FitCanvas();
        }
    }

    public void FitCanvas()
    {
        if (canvas == null || followedCamera == null)
        {
            Debug.LogError("Canvas is not assigned, Camera is not assigned, or Camera is not orthographic.");
            return;
        }

        float frustumHeight = followedCamera.orthographicSize * 2;
        float frustumWidth = frustumHeight * followedCamera.aspect;

        canvasTransform.sizeDelta = new Vector2(frustumWidth, frustumHeight);

        canvas.transform.position = followedCamera.transform.position + followedCamera.transform.forward;
        canvas.transform.rotation = followedCamera.transform.rotation;
    }
}
