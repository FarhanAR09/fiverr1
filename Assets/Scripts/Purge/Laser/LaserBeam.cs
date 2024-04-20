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
        if (Equals(collision.gameObject, PlayerInput.GOInstance) && !PlayerInput.GOInstance.GetComponent<PlayerInput>().Lost)
        {
            Debug.Log("Player lost");
            GameEvents.OnPlayerLose.Publish(false);
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
            Destroy(gameObject, activeDuration);
        }
        StartCoroutine(LaserTiming());
    }
}
