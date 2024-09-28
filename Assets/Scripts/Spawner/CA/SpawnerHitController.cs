using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    public class SpawnerHitController : MonoBehaviour, ICAHittable, ICAHurtable, IHealthOwner
    {
        public GameObject Owner => gameObject;

        private SpawnerController spawner;

        //Health
        [SerializeField]
        private Health health = new(100f);
        public float CurrentHealth => health.CurrentHealth;
        public float MaxHealth => health.MaxHealth;
        public UnityAction<float> OnHealthUpdated { get => health.OnHealthUpdated; set => health.OnHealthUpdated = value; }

        public UnityAction<bool> OnSpawnerEnabledStateChanged;

        private void Awake()
        {
            TryGetComponent(out spawner);
        }

        private void Start()
        {
            if (spawner != null)
            {
                spawner.enabled = false;
            }
        }

        public bool TryHit()
        {
            return health.CurrentHealth > 0f;
        }

        public void Hurt(float baseDamage)
        {
            health.TakeDamage(baseDamage);
            if (health.CurrentHealth <= 0f && spawner != null && !spawner.enabled)
            {
                if (spawner != null)
                {
                    spawner.enabled = true;
                }
                OnSpawnerEnabledStateChanged?.Invoke(true);
            }
        }
    }
}