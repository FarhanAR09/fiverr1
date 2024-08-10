using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MLLeakTracker : MonoBehaviour
{
    public static MLLeakTracker Instance { get; private set; }

    [field: SerializeField]
    public int LeakedMemory { get; private set; }
    [field: SerializeField]
    public int MaxMemory { get; private set; } = 100;

    private readonly float addLeakDuration = 4f;
    private float addLeakTime = 0f;

    public UnityAction<int> OnMemoryLeakUpdated;

    private void OnEnable()
    {
        GameEvents.OnMLMistakesUpdated.Add(AddLeakByMistakes);
    }

    private void OnDisable()
    {
        GameEvents.OnMLMistakesUpdated.Remove(AddLeakByMistakes);
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

    private void FixedUpdate()
    {
        if (addLeakTime < addLeakDuration)
        {
            addLeakTime += Time.fixedDeltaTime;
        }
        else
        {
            addLeakTime = 0f;
            AddLeak(1);
        }
    }

    private void AddLeakByMistakes(int mistakes)
    {
        AddLeak(1 + Mathf.FloorToInt(10 * Mathf.Log(mistakes + 1, 32)));
        print("Leaked Memory: " + LeakedMemory);
    }

    private void AddLeak(int amount)
    {
        LeakedMemory += amount;
        OnMemoryLeakUpdated?.Invoke(LeakedMemory);
        if (LeakedMemory >= MaxMemory)
        {
            print("-----=====| YOU LOSE |=====-----");
            GameEvents.OnMLLost.Publish(true);
        }
    }
}
