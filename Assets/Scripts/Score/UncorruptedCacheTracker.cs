using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncorruptedCacheTracker : MonoBehaviour
{
    public static UncorruptedCacheTracker Instance { get; private set; }

    private readonly List<ScorePellet> bits = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnBitInitialized.Add(TrackBit);
        GameEvents.OnLevelUp.Add(ResetBitsList);
    }

    private void OnDisable()
    {
        GameEvents.OnBitInitialized.Remove(TrackBit);
        GameEvents.OnLevelUp.Remove(ResetBitsList);
    }

    private void TrackBit(ScorePellet pellet)
    {
        if (!pellet.Corrupted)
            bits.Add(pellet);
    }

    private void ResetBitsList(bool _)
    {
        bits.Clear();
    }

    /// <summary>
    /// Try to return bit. Might return null if no bit exists.
    /// </summary>
    /// <returns></returns>
    public ScorePellet TryGetBit()
    {
        //Remove nulls
        int count = bits.Count;
        Stack<int> toBeRemovedIndexes = new();
        for (int i = 0; i < count; i++)
        {
            if (bits[i] == null)
            {
                toBeRemovedIndexes.Push(i);
            }
        }
        while (toBeRemovedIndexes.Count > 0)
        {
            bits.RemoveAt(toBeRemovedIndexes.Pop());
        }

        //Retrieve first pellet
        if (bits.Count > 0 && bits[0] != null)
        {
            return bits[0];
        }

        //Fail (probably empty list)
        return null;
    }
}
