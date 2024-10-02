using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    [DefaultExecutionOrder(-1)]
    public class Threshold : MonoBehaviour
    {
        private static Threshold _instance;
        public static Threshold Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning("Threshold instance is null");
                }
                return _instance;
            }
            set => _instance = value;
        }

        [field: SerializeField]
        public float Amount { get; private set; } = 0f;
        [field: SerializeField]
        public float Limit { get; private set; } = 0f;
        private bool limitReached = false;
        public UnityAction<float> OnThresholdUpdated { get; set; }
        public UnityAction OnLimitReached { get; set; }

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

        public void SetLimit(float limit)
        {
            Limit = limit;
            limitReached = false;
        }

        public void Add(float amount)
        {
            Amount += amount;
            OnThresholdUpdated?.Invoke(Amount);
            if (!limitReached && Amount >= Limit)
            {
                limitReached = true;
                OnLimitReached?.Invoke();
            }
        }
    }
}