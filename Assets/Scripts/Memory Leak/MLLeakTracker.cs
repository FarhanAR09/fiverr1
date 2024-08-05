using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLLeakTracker : MonoBehaviour
{
    public static MLLeakTracker Instance { get; private set; }

    [field: SerializeField]
    public int LeakedMemory { get; private set; }
    [field: SerializeField]
    public int MaxMemory { get; private set; } = 100;

    private void OnEnable()
    {
        GameEvents.OnMLMistakesUpdated.Add(AddLeak);
    }

    private void OnDisable()
    {
        GameEvents.OnMLMistakesUpdated.Remove(AddLeak);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        LeakedMemory = 0;
    }

    private void AddLeak(int mistakes)
    {
        LeakedMemory += 1 + Mathf.FloorToInt(10 * Mathf.Log(mistakes + 1, 32));
        print("Leaked Memory: " + LeakedMemory);
        if (LeakedMemory >= MaxMemory)
        {
            print("-----=====| YOU LOSE |=====-----");
            GameEvents.OnMLLost.Publish(true);
        }
    }
}
