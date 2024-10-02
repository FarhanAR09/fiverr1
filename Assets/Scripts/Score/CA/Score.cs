using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    [DefaultExecutionOrder(-1)]
    public class Score : MonoBehaviour
    {
        //Singleton
        private static Score _instance;
        public static Score Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("Score Instance is null");
                }
                return _instance;
            }
            set => _instance = value;
        }

        public UnityAction<int> OnScoreUpdated { get; set;}

        [field: SerializeField]
        public int Amount { get; private set; }

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

        public void AddScore(int amount)
        {
            Amount += amount;
            OnScoreUpdated?.Invoke(Amount);
        }

        public void RemoveScore(int amount)
        { 
            AddScore(-amount);
        }
    }
}