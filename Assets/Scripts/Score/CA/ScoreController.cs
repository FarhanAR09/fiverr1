using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class ScoreController : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnCAEnemyDeath.Add(AddScore);
        }

        private void OnDisable()
        {
            GameEvents.OnCAEnemyDeath.Remove(AddScore);
        }

        private void AddScore(Enemy enemy)
        {
            Score.Instance.AddScore(
                Mathf.RoundToInt(TypeToMultiplier(enemy.Type) *
                (FactorNumberTracker.Instance != null ?
                    FactorNumberTracker.Instance.Count :
                    1f)));
        }

        private float TypeToMultiplier(EnemyType type)
        {
            return type switch
            {
                EnemyType.ElectricGhost => 1f,
                EnemyType.QuantumGhost => 1f,
                EnemyType.Spider => 1f,
                EnemyType.TrojanHorse => 1f,
                EnemyType.CorruptedGhost => 1f,
                _ => 1f,
            };
        }
    }
}