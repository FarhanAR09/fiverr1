using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScoreCollector
{
    public bool CanEatUncorrupted { get; }
    public bool CanEatCorrupted { get; }
    public bool EatingBitProduceScore { get; }
    public bool CanCorruptBit { get; }
    public void NotifyBitEaten();
}