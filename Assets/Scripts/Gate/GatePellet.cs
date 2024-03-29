using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class GatePellet : MonoBehaviour
{
    public UnityEvent OnCollected { get; private set; } = new();

    private void Awake()
    {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.25f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            OnCollected.Invoke();
            OnCollected.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}
