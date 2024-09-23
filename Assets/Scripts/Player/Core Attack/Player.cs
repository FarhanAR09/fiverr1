using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreAttack
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour, ICAHittable, ICAHurtable
    {
        //Singleton
        private static CoreAttack.Player instance;
        public static CoreAttack.Player Instance {
            get
            {
                if (instance == null)
                    Debug.LogWarning("Core Attack Player Instance is null");
                return instance;
            }
            private set => instance = value;
        }

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
        }
    }
}
