using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private bool beenSetup = false;

    [SerializeField]
    private float prepareDuration = 2f, activeDuration = 1.5f;
    private bool isHorizontal = false;
    private Vector2 targetPosition;

    private Animator animator;
    private BoxCollider2D boxCollider;

    [SerializeField]
    private AudioClip shootSFX;

    private void Awake()
    {
        TryGetComponent(out animator);

        TryGetComponent(out boxCollider);
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
            boxCollider.enabled = false;
        }
    }

    private void Start()
    {
        StartCoroutine(LaserShooting());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Detect player
        if (collision.TryGetComponent(out IEnemyHurtable hurtable))
        {
            //GameEvents.OnPlayerLose.Publish(false);
            hurtable.TryHurt();
        }
    }

    public void StartLaser(bool isHorizontal, Vector2 targetPosition)
    {
        beenSetup = true;
        this.isHorizontal = isHorizontal;
        this.targetPosition = targetPosition;
    }

    private IEnumerator LaserShooting()
    {
        yield return new WaitUntil(() => beenSetup);

        IEnumerator LaserTiming()
        {
            //Start preparing
            if (animator != null)
                animator.Play("laserbeam_prepare");

            float moveDuration = 0.5f;
            float moveTime = 0;

            transform.localRotation = Quaternion.Euler(0, 0, isHorizontal ? 0 : 90);
            Vector2 initPos = isHorizontal ? new Vector2(-160, targetPosition.y) : new Vector2(targetPosition.x, -160);

            while (true)
            {
                if (moveTime < moveDuration)
                {
                    moveTime += Time.fixedDeltaTime;
                    transform.position = Vector2.Lerp(initPos, targetPosition, moveTime / moveDuration);
                }
                else
                {
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(prepareDuration - moveDuration);

            //Start shooting
            if (boxCollider != null)
                boxCollider.enabled = true;
            if (animator != null)
                animator.Play("laserbeam_active");
            if (shootSFX != null && SFXController.Instance != null)
                SFXController.Instance.RequestPlay(shootSFX, 19000, timePitching: false);
            Destroy(gameObject, activeDuration);
        }
        StartCoroutine(LaserTiming());
    }
}
