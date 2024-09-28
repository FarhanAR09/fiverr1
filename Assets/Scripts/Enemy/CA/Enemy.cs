using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    public class Enemy : MonoBehaviour, ICAHittable, ICAHurtable, IHealthOwner
    {
        //Hittable
        public GameObject Owner => gameObject;

        //Health
        [SerializeField]
        private Health health;
        //IHealthOwner
        public float CurrentHealth => health.CurrentHealth;
        public float MaxHealth => health.MaxHealth;
        public UnityAction<float> OnHealthUpdated {
            get => health.OnHealthUpdated;
            set => health.OnHealthUpdated = value;
        }

        [field: SerializeField]
        public EnemyType Type { get; private set; } = EnemyType.ElectricGhost;

        public bool TryHit()
        {
            return true;
        }

        public void Hurt(float baseDamage)
        {
            health.TakeDamage(baseDamage);
            if (health.CurrentHealth <= 0)
            {
                GameEvents.OnCAEnemyDeath.Publish(this);
                Destroy(gameObject);
            }
        }
    }
}
