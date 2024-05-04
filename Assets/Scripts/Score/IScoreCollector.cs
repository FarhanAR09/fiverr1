using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IScoreCollector
{
    public bool CanEatUncorrupted { get; }
    public bool CanEatCorrupted { get; }
}