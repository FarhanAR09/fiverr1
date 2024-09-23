using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public static class EnemyTypeToKey
    {
        public static Dictionary<EnemyType, string> EnemyTypeToKeyMapping { get; private set; } = new Dictionary<EnemyType, string>
        {
            {EnemyType.ElectricGhost, GameConstants.CAELECTRICGHOSTKEY}
        };
    }
}
