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

    [SerializeField]
    private AudioClip pickupSFX;

    private bool corrupted = false;

    private void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;

        if (TryGetComponent(out Animator _animator))
            animator = _animator;
    }

    private void Start()
    {
        if (animator != null)
            animator.Play("pellet_spawn");
    }

    private void OnEnable()
    {
        GameEvents.OnPurgeWarning.Add(DisablePelletPurge);
        GameEvents.OnPurgeFinished.Add(EnablePelletPurge);
    }

    private void OnDisable()
    {
        GameEvents.OnPurgeWarning.Remove(DisablePelletPurge);
        GameEvents.OnPurgeFinished.Remove(EnablePelletPurge);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canBePicked && collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            if (animator != null)
                animator.Play("pellet_picked");
            canBePicked = false;
            ScoreCounter.AddScore(10 * (corrupted ? -2 : 1));

            if (pickupSFX != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(pickupSFX, 10000, volumeMultiplier: 0.15f, pitchMultiplier: corrupted ? 0.5f : 1f);
            }

            Destroy(gameObject, animator != null ? animator.GetCurrentAnimatorStateInfo(0).length : 0);
        }
    }

    public void Stun(float duration)
    {
        if (spriteRenderer != null)
        {
            //spriteRenderer.enabled = false;
            spriteRenderer.color = Color.red;
        }
        float animationTime = 0;
        if (psDischarge != null)
        {   
            psDischarge.Emit(1);
            animationTime = psDischarge.main.startLifetime.constant;
        }
        corrupted = true;
        //Destroy(gameObject, animationTime);
    }

    private void DisablePelletPurge(bool _)
    {
        canBePicked = false;
        //if (spriteRenderer != null)
        //    spriteRenderer.enabled = false;

        if (animator != null)
            animator.Play("pellet_picked");
    }

    private void EnablePelletPurge(bool _)
    {
        canBePicked = true;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (animator != null)
            animator.Play("pellet_spawn");
    }
}
