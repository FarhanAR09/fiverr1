using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class GatePellet : MonoBehaviour
{
    public UnityEvent OnCollected { get; private set; } = new();

    [SerializeField]
    private AudioClip pickedSFX;

    private void Awake()
    {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.25f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            if (pickedSFX != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(pickedSFX, 15000);
            }

            OnCollected.Invoke();
            OnCollected.RemoveAllListeners();
            Destroy(gameObject);
        }
    }
}
