using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class Enemy : MonoBehaviour, ICAHittable, ICAHurtable
    {
        //Hittable
        public GameObject Owner => gameObject;

        [SerializeField]
        private Health health;

        public void Hit()
        {
            //Nothing
        }

        public void Hurt(float baseDamage)
        {
            health.TakeDamage(baseDamage);
            if (health.CurrentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
