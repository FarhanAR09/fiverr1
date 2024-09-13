using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAGenericBullet : CAProjectile
{
    public override void Disappear()
    {
        base.Disappear();
    }

    public override void Fire(Vector2 velocity)
    {
        base.Fire(velocity);
    }

    public override void OnHit(ICAHittable hittable)
    {
        base.OnHit(hittable);
        hittable.Hit();
        if (hittable.Owner != null && hittable.Owner.TryGetComponent(out ICAHurtable hurtable))
        {
            hurtable.Hurt(10f);
        }

        Disappear();
    }
}
