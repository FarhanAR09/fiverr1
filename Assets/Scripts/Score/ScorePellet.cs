using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ScorePellet : MonoBehaviour, IStunnable
{
    private CircleCollider2D circleCollider;
    private Animator animator;
    private bool canBePicked = true;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private ParticleSystem psDischarge;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        if (TryGetComponent(out Animator _animator))
            animator = _animator;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBePicked && collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            if (animator != null)
                animator.Play("pellet_picked");
            canBePicked = false;
            ScoreCounter.AddScore(10);
            Destroy(gameObject, animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : 0);
        }
    }

    public void Stun(float duration)
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        float animationTime = 0;
        if (psDischarge != null)
        {
            psDischarge.Emit(1);
            animationTime = psDischarge.main.duration;
        }
        Destroy(gameObject, animationTime);
    }
}
