using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    public class Enemy : MonoBehaviour, ICAHittable, ICAHurtable
    {
        public GameObject Owner => gameObject;

        public void Hit()
        {
            
        }

        public void Hurt(float baseDamage)
        {
            
        }
    }
}
