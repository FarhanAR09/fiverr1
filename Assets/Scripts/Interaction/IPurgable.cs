using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurgable
{
    public bool TryPurge();
}