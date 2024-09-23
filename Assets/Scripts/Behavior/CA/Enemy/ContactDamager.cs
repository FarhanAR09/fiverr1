using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreAttack;

namespace CoreAttack
{
    public class ContactDamager : MonoBehaviour
    {
        public bool ContactDamagerEnabled { get; set; } = true;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (ContactDamagerEnabled)
            {
                if (collision.gameObject.Equals(Player.Instance.gameObject))
                {
                    if (Player.Instance.TryHit())
                    {
                        Player.Instance.Hurt(10f);
                    }
                }
            }
        }
    }
}