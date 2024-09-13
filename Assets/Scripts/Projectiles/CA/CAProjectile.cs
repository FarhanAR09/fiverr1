using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CAProjectile : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        TryGetComponent(out rb);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICAHittable hittable))
        {
            OnHit(hittable);
        }
    }

    public virtual void Fire(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }

    public virtual void OnHit(ICAHittable hittable) 
    {
        //Empty
    }

    public virtual void Disappear()
    {
        //TODO: implement bullet pool
        Destroy(gameObject);
    }
}
