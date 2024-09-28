using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CoreAttack
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, ICAHittable, ICAHurtable, IHealthOwner
    {
        //Singleton
        private static Player instance;
        public static Player Instance {
            get
            {
                if (instance == null)
                    Debug.LogWarning("Core Attack Player Instance is null");
                return instance;
            }
            private set => instance = value;
        }

        //Health
        [SerializeField]
        private Health health;
        //IHealthOwner
        public float CurrentHealth => health.CurrentHealth;
        public float MaxHealth => health.MaxHealth;
        public UnityAction<float> OnHealthUpdated { get => health.OnHealthUpdated; set => health.OnHealthUpdated = value; }

        //Hittable
        public GameObject Owner => gameObject;

        private void Awake()
        {
            //Singleton
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        public bool TryHit()
        {
            return true;
        }

        public void Hurt(float baseDamage)
        {
            print("Player: ouch");
            health.TakeDamage(baseDamage);
            if (health.CurrentHealth <= 0f)
            {
                print("Player: dead");
            }
        }
    }
}
