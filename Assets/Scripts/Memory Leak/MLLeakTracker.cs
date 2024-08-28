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

    private int calculatedMistakes = 0;

    private void OnEnable()
    {
        GameEvents.OnMLMistakesUpdated.Add(AddLeakByMistakes);
        GameEvents.OnMLFreezeStateUpdated.Add(SetFrozenState);
        GameEvents.OnMLCardsPaired.Add(ReduceLeakByCombo);
        GameEvents.OnMLCorruptCardsPaired.Add(AddLeakByCorruptPair);
    }

    private void OnDisable()
    {
        GameEvents.OnMLMistakesUpdated.Remove(AddLeakByMistakes);
        GameEvents.OnMLFreezeStateUpdated.Remove(SetFrozenState);
        GameEvents.OnMLCardsPaired.Remove(ReduceLeakByCombo);
        GameEvents.OnMLCorruptCardsPaired.Remove(AddLeakByCorruptPair);
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

        MaxMemory = PlayerPrefs.GetInt(GameConstants.MLFSMAXLEAK, MaxMemory);
    }

    private void FixedUpdate()
    {
        if (!isFrozen && MLPlayManager.Instance != null && MLPlayManager.Instance.CheckMode(MLGameMode.Classic, MLGameMode.Endless))
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
        if (lost || isFrozen)
            return;

        if (MLPlayManager.Instance != null)
        {
            if (MLPlayManager.Instance.CheckMode(MLGameMode.Classic, MLGameMode.Endless))
            {
                float multiplier = 1;
                if (MLPlayManager.Instance != null)
                {
                    multiplier = MLPlayManager.Instance.Difficulty switch
                    {
                        1 => 1f,
                        2 => 1.2f,
                        3 => 1.4f,
                        4 => 1.8f,
                        5 => 2f,
                        _ => 1f,
                    };
                }
                calculatedMistakes++;
                AddLeak(1 + Mathf.FloorToInt(multiplier * Mathf.Log(calculatedMistakes + 1, 32)));
            }
            else //Defaults to trial
            {
                calculatedMistakes++;
                SetLeak(MaxMemory * calculatedMistakes / MLMainMenuFeatureSwitches.DebugTrialMaxMistakes);
            }
        }
    }

    private void AddLeak(int amount)
    {
        if (lost)
            return;
        LeakedMemory = Mathf.Max(0, LeakedMemory + amount);
        OnMemoryLeakUpdated?.Invoke(LeakedMemory);
        if (LeakedMemory >= MaxMemory)
        {
            lost = true;
            
            GameEvents.OnMLLost.Publish(true);
        }
    }

    private void SetLeak(int amount)
    {
        if (lost)
            return;

        LeakedMemory = Mathf.Max(0, amount);
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
        if (lost || !(MLPlayManager.Instance != null && MLPlayManager.Instance.CheckMode(MLGameMode.Classic, MLGameMode.Endless)))
            return;

        if (MemoryTracker.Instance != null)
        {
            int reduce = -Mathf.CeilToInt(20 * Mathf.Log(0.25f * (MemoryTracker.Instance.Combo - 1) + 1));
            AddLeak(reduce);
        }
    }

    private void AddLeakByCorruptPair(CardPairArgument arg)
    {
        if (lost || isFrozen || !(MLPlayManager.Instance != null && MLPlayManager.Instance.CheckMode(MLGameMode.Classic, MLGameMode.Endless)))
            return;

        if (arg.card1.Corrupted && arg.card2.Corrupted)
        {
            AddLeak(30);
        }
        else if (arg.card1.Corrupted)
        {
            AddLeak(10);
        }
        else if (arg.card2.Corrupted)
        {
            AddLeak(10);
        }
    }
}
