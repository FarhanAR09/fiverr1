using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CircleCollider2D))]
public class GatePellet : MonoBehaviour
{
    private enum State
    {
        DontCollect,
        Available,
        Activated
    }
    private State state = State.DontCollect;

    public UnityEvent<int> OnCollected { get; private set; } = new();

    [SerializeField]
    private AudioClip pickedSFX;

    private bool beenSetUp = false;
    private int order = -1;
    private GateDisplay gateDisplay;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite deactivatedSprite;

    [SerializeField]
    private ParticleSystem psExplode;

    private bool inPurge = false;

    private Animator animator;

    private bool featureActivated = true;

    private void Awake()
    {
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.25f;

        TryGetComponent(out animator);
    }

    private void OnEnable()
    {
        GameEvents.OnGatesSequenceUpdate.Add(UpdateGateState);
        GameEvents.OnPurgeWarning.Add(PurgeShiverMeTimbers);
        GameEvents.OnPurgeStarted.Add(DisableOnPurge);
        GameEvents.OnPurgeFinished.Add(EnableOnPurge);
        GameEvents.OnSwitchSequentialGates.Add(FeatureSwitchState);
    }

    private void OnDisable()
    {
        GameEvents.OnGatesSequenceUpdate.Remove(UpdateGateState);
        GameEvents.OnPurgeWarning.Remove(PurgeShiverMeTimbers);
        GameEvents.OnPurgeStarted.Remove(DisableOnPurge);
        GameEvents.OnPurgeFinished.Remove(EnableOnPurge);
        GameEvents.OnSwitchSequentialGates.Remove(FeatureSwitchState);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (featureActivated && beenSetUp && !inPurge && state != State.Activated)
        {
            if (collision.gameObject.Equals(PlayerInput.GOInstance))
            {
                if (pickedSFX != null && SFXController.Instance != null)
                {
                    SFXController.Instance.RequestPlay(pickedSFX, 15000);
                }

                OnCollected.Invoke(order);
            }
        }
    }

    public void Setup(int order, GateDisplay gateDisplay)
    {
        this.order = order;
        this.gateDisplay = gateDisplay;

        beenSetUp = true;
    }

    private void UpdateGateState(int currentOrder)
    {
        if (currentOrder > order)
        {
            state = State.Activated;
            if (spriteRenderer != null)
            {
                if (gateDisplay != null)
                {
                    spriteRenderer.sprite = gateDisplay.activeSprite;
                    spriteRenderer.color = Color.green;
                }
                if (spriteRenderer.material != null)
                {
                    spriteRenderer.material.SetFloat("_Intensity", 2);
                } 
            }
        }
        else if (currentOrder == order)
        {
            state = State.Available;
            if (spriteRenderer != null)
            {
                if (gateDisplay != null)
                {
                    spriteRenderer.sprite = deactivatedSprite;
                    spriteRenderer.color = Color.white;
                }
                if (spriteRenderer.material != null)
                {
                    spriteRenderer.material.SetFloat("_Intensity", 3);
                }
            }
        }
        else
        {
            state = State.DontCollect;
            if (spriteRenderer != null)
            {
                if (gateDisplay != null)
                {
                    spriteRenderer.sprite = deactivatedSprite;
                    spriteRenderer.color = new Color(0.7f, 0.7f, 0.7f, 1f);
                }
                if (spriteRenderer.material != null)
                {
                    spriteRenderer.material.SetFloat("_Intensity", 1);
                }
            }
        }
    }

    private void PurgeShiverMeTimbers(bool _)
    {
        if (animator != null)
        {
            animator.Play("gate_shaking");
        }
    }

    private void DisableOnPurge(bool _)
    {
        inPurge = true;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        if (psExplode != null)
        {
            var main = psExplode.main;
            main.startColor = state == State.Activated ? Color.green : Color.white;
            psExplode.Emit(15);
        }
    }

    private void EnableOnPurge(bool _)
    {
        inPurge = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        if (animator != null)
        {
            animator.Play("gate_noshake");
        }
    }

    private void FeatureSwitchState(bool active)
    {
        featureActivated = active;
        if (spriteRenderer != null)
            spriteRenderer.enabled = active;
    }
}
