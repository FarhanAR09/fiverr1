using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyHurtable
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Is hurting successful?</returns>
    public bool TryHurt();
}
