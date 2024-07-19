using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPController : MonoBehaviour
{
    private GameObject goHitbox;
    private Hitbox hitbox;
    private CircleCollider2D circleCollider;

    private void Awake()
    {
        goHitbox = new GameObject("EMP Hitbox", typeof(Hitbox), typeof(CircleCollider2D));

        goHitbox.transform.parent = transform;
        goHitbox.transform.localPosition = Vector3.zero;

        circleCollider = goHitbox.GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 4f;

        hitbox = goHitbox.GetComponent<Hitbox>();
        hitbox.OnDetected.AddListener(OnDetected);
    }

    private void OnDetected(Collider2D collider)
    {
        if (collider.TryGetComponent(out IStunnable stunnable))
        {
            stunnable.Stun(3f);
        }
    }
}
