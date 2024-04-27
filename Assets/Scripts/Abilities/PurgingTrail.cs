using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgingTrail : MonoBehaviour
{
    [SerializeField]
    private Hitbox hitbox;

    private void OnEnable()
    {
        if (hitbox != null)
            hitbox.OnDetected.AddListener(HandleOnDetected);
    }

    private void OnDisable()
    {
        if (hitbox != null)
            hitbox.OnDetected.RemoveListener(HandleOnDetected);
    }

    private void HandleOnDetected(Collider2D collider)
    {
        if (collider.TryGetComponent(out IPurgable purgable))
        {
            purgable.Purge();
        }
    }
}
