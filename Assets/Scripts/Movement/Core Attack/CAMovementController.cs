using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CAMovementController : MonoBehaviour
{
    private Rigidbody2D rb;
    [field: SerializeField]
    public float Speed { get; set; } = 5f;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    private void Start()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void MoveTo(Vector2 position)
    {
        if (rb != null)
        {
            Vector2 deviation = Speed * Time.fixedDeltaTime * (position - (Vector2)transform.position).normalized;
            rb.MovePosition((Vector2)transform.position + deviation);
        }
    }
}
