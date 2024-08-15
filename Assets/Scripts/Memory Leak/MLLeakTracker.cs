using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool isFrozen = false;

    public UnityAction<int> OnMemoryLeakUpdated;

    private bool lost = false;

    private void OnEnable()
    {
        GameEvents.OnMLMistakesUpdated.Add(AddLeakByMistakes);
        GameEvents.OnMLFreezeStateUpdated.Add(SetFrozenState);
        GameEvents.OnCardsPaired.Add(ReduceLeakByCombo);
    }

    private void OnDisable()
    {
        GameEvents.OnMLMistakesUpdated.Remove(AddLeakByMistakes);
        GameEvents.OnMLFreezeStateUpdated.Remove(SetFrozenState);
        GameEvents.OnCardsPaired.Remove(ReduceLeakByCombo);
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
        if (!isFrozen)
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
    }

    private void AddLeakByMistakes(int mistakes)
    {
        int multiplier = 10;
        if (MLPlayManager.Instance != null)
        {
            multiplier = MLPlayManager.Instance.CurrentLevel switch
            {
                1 => 10,
                2 => 12,
                3 => 14,
                4 => 17,
                5 => 20,
                _ => 10,
            };
        }
        AddLeak(1 + Mathf.FloorToInt(multiplier * Mathf.Log(mistakes + 1, 32)));
    }

    private void AddLeak(int amount)
    {
        if (lost)
            return;

        if (LeakedMemory + amount > 0)
        {
            LeakedMemory += amount;
        }
        else
        {
            LeakedMemory = 0;
        }
        OnMemoryLeakUpdated?.Invoke(LeakedMemory);
        if (LeakedMemory >= MaxMemory)
        {
            lost = true;
            print("-----=====| YOU LOSE |=====-----");
            GameEvents.OnMLLost.Publish(true);
        }
    }

    private void SetFrozenState(bool frozen)
    {
        isFrozen = frozen;
    }

    private void ReduceLeakByCombo(CardPairArgument _)
    {
        if (MemoryTracker.Instance != null)
        {
            int reduce = -Mathf.CeilToInt(20 * Mathf.Log(0.25f * (MemoryTracker.Instance.Combo - 1) + 1));
            AddLeak(reduce);
        }
    }
}
