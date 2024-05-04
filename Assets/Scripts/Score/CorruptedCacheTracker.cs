using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptedCacheTracker : MonoBehaviour
{
    public static CorruptedCacheTracker Instance { get; private set; }

    private readonly List<ScorePellet> corruptedBits = new();

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
        GameEvents.OnBitCorrupted.Add(TrackCorruptedPellet);
        GameEvents.OnLevelUp.Add(ResetBitsList);
    }

    private void OnDisable()
    {
        GameEvents.OnBitCorrupted.Remove(TrackCorruptedPellet);
        GameEvents.OnLevelUp.Remove(ResetBitsList);
    }

    private void TrackCorruptedPellet(ScorePellet pellet)
    {
        corruptedBits.Add(pellet);
    }

    private void ResetBitsList(bool _)
    {
        corruptedBits.Clear();
    }

    /// <summary>
    /// Try to return a corrupted bits. Might return null if no corrupted bit exists.
    /// </summary>
    /// <returns></returns>
    public ScorePellet TryGetCorruptedPellet()
    {
        //Remove nulls
        int corruptedCount = corruptedBits.Count;
        Stack<int> toBeRemovedIndexes = new();
        for (int i = 0; i < corruptedCount; i++)
        {
            if (corruptedBits[i] == null)
            {
                toBeRemovedIndexes.Push(i);
            }
        }
        while (toBeRemovedIndexes.Count > 0)
        {
            corruptedBits.RemoveAt(toBeRemovedIndexes.Pop());
        }

        //Retrieve first pellet
        if (corruptedBits.Count > 0 && corruptedBits[0] != null)
        {
            return corruptedBits[0];
        }

        //Fail (probably empty list)
        return null;
    }
}
