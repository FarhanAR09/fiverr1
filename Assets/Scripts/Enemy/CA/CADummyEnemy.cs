using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CADummyEnemy : MonoBehaviour, ICAHittable, ICAHurtable
{
    public GameObject Owner => gameObject;

    public void Hit()
    {
        
    }

    public void Hurt(float baseDamage)
    {
        
    }
}
