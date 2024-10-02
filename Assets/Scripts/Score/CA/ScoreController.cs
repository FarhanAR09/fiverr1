using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

namespace CoreAttack
{
    public class ScoreController : MonoBehaviour
    {
        private static ScoreController _instance;
        public static ScoreController Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("ScoreController instance is NULL");
                }
                return _instance;
            }
            set => _instance = value;
        }

        private void OnEnable()
        {
            GameEvents.OnCAEnemyDeath.Add(AddScore);
        }

        private void OnDisable()
        {
            GameEvents.OnCAEnemyDeath.Remove(AddScore);
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void AddScore(Enemy enemy)
        {
            if (Score.Instance != null)
            {
                Score.Instance.AddScore(
                Mathf.RoundToInt(TypeToMultiplier(enemy.Type) *
                (FactorNumberTracker.Instance != null ?
                    FactorNumberTracker.Instance.Count :
                    1f)));
            }
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