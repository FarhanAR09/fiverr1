using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    [Serializable]
    public class Health
    {
        [SerializeField]
        private float _currentHealth;
        [SerializeField]
        private float _maxHealth;

        public UnityAction<float> OnHealthUpdated { get; set; }

        public Health(float maxHealth, float currentHealth = -1)
        {
            _maxHealth = maxHealth;
            if (currentHealth < 0 || currentHealth > maxHealth)
                _currentHealth = maxHealth;
            else
                _currentHealth = currentHealth;
        }

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnHealthUpdated?.Invoke(_currentHealth);
        }

        public void Heal(float amount)
        {
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth + amount);
            OnHealthUpdated?.Invoke(_currentHealth);
        }
    }
}