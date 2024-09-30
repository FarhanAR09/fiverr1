using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    public class FactorNumberTracker : MonoBehaviour
    {
        //Singleton
        private static FactorNumberTracker _instance;
        public static FactorNumberTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("FactorNumberTracker instance is null");
                }
                return _instance;
            }
            private set => _instance = value;
        }

        public int Count { get; private set; } = 0;

        public UnityAction<int> OnCounterUpdated { get; set; }

        private void OnEnable()
        {
            GameEvents.OnCAEnemySpawned.Add(AddCounter);
            GameEvents.OnCAEnemyDeath.Add(DecreaseCounter);
        }

        private void OnDisable()
        {
            GameEvents.OnCAEnemySpawned.Remove(AddCounter);
            GameEvents.OnCAEnemyDeath.Remove(DecreaseCounter);
        }

        private void Awake()
        {
            //Singleton
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void AddCounter(Enemy _)
        {
            Count++;
            OnCounterUpdated?.Invoke(Count);
        }

        private void DecreaseCounter(Enemy _)
        {
            Count--;
            OnCounterUpdated?.Invoke(Count);
        }
    }
}