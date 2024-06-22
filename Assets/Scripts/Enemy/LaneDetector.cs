using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LaneDetector : MonoBehaviour
{
    public UnityAction<LaneDetectionData> OnPlayerDetected { get; set; }
    public bool IsVertical { get; private set; }

    private BoxCollider2D coll;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        coll.isTrigger = true;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
    }

    private void OnDestroy()
    {
        OnPlayerDetected = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            OnPlayerDetected?.Invoke(new LaneDetectionData(IsVertical, collision.gameObject));
        }
    }

    public void Setup(Vector2 startPos, Vector2 endPos, float cellSize, bool isVertical)
    {
        transform.position = Vector3.Lerp(startPos, endPos, 0.5f) + cellSize / 2f * new Vector3(1, 1);
        coll.size =
            isVertical ?
            new Vector2(1, 1 + Mathf.Abs(startPos.y - endPos.y)):
            new Vector2(1 + Mathf.Abs(startPos.x - endPos.x), 1);
        IsVertical = isVertical;
    }
}

public class LaneDetectionData
{
    public bool IsVertical { get; private set; }
    public GameObject DetectedGO { get; private set; }

    public LaneDetectionData(bool isVertical, GameObject detectedObject)
    {
        IsVertical = isVertical;
        DetectedGO = detectedObject;
    }
}
