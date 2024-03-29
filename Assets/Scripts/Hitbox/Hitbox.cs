using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    private Collider2D _collider;
    public UnityEvent<Collider2D> OnDetected { get; private set; } = new();

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnDetected.Invoke(collision);
    }
}
