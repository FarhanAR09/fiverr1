using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPatrolWatcher : MonoBehaviour
{
    private CircleCollider2D circleCollider;
    public UnityEvent OnPlayerDetected { get; private set; } = new();

    private void Awake()
    {
        circleCollider = gameObject.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 3;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(PlayerMovement.GOInstance))
        {
            OnPlayerDetected.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnPlayerDetected.RemoveAllListeners();
    }
}
