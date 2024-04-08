using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPatrol : MonoBehaviour
{
    public UnityEvent OnPlayerDetected { get; private set; } = new();
    private EnemyPatrolWatcher watcher;

    private void Awake()
    {
        //Instantiates patrol watcher
        watcher = new GameObject("Patrol Watcher", typeof(EnemyPatrolWatcher), typeof(Rigidbody2D)).GetComponent<EnemyPatrolWatcher>();
        watcher.GetComponent<Rigidbody2D>().isKinematic = true;
        watcher.transform.parent = transform;
        watcher.transform.localPosition = Vector3.zero;
    }

    private void OnEnable()
    {
        watcher.OnPlayerDetected.AddListener(InvokePlayerDetected);
    }

    private void OnDisable()
    {
        watcher.OnPlayerDetected.RemoveListener(InvokePlayerDetected);
    }

    private void OnDestroy()
    {
        OnPlayerDetected.RemoveAllListeners();
    }

    private void InvokePlayerDetected()
    {
        OnPlayerDetected.Invoke();
    }
}
