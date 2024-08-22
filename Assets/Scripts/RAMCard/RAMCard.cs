using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(StateMachine))]
public class RAMCard : MonoBehaviour
{
    //TODO: cuma coba2 pairing, move to data
    [field: SerializeField]
    public int CardNumber { get; private set; } = 0;
    public bool Corrupted { get; private set; } = false;

    public UnityAction OnClicked, PairRequestCalled, PutDownRequestCalled;


    private StateMachine stateMachine;
    public CardDownState DownState { get; private set; }
    public CardUpState UpState { get; private set; }
    public CardPairedState PairedState { get; private set; }
    public CardPeekedState PeekedState { get; private set; }

    [SerializeField]
    private TMP_Text nameDisplay;

    [SerializeField]
    private SpriteRenderer background, icon;

    [SerializeField]
    private ParticleSystem psExplode;

    private bool assetBeenSetup = false;

    private void OnEnable()
    {
        GameEvents.OnMLCardsFailPairing.Add(EmitFailPairParticles);
    }

    private void OnDisable()
    {
        GameEvents.OnMLCardsFailPairing.Remove(EmitFailPairParticles);
    }

    private void Awake()
    {
        TryGetComponent(out stateMachine);
        if (stateMachine != null)
        {
            DownState = new(this, stateMachine);
            UpState = new(this, stateMachine);
            PairedState = new(this, stateMachine);
            PeekedState = new(this, stateMachine);
        }
    }

    private void OnMouseDown()
    {
        //print(name + " clicked");
        OnClicked?.Invoke();
    }

    public void PairCard()
    {
        //print(name + " is being paired");
        PairRequestCalled?.Invoke();
    }

    public void PutDownCard()
    {
        PutDownRequestCalled?.Invoke();
    }

    public void SetupNumber(int cardNumber, bool corrupted = false)
    {
        CardNumber = cardNumber;
        Corrupted = corrupted;

        if (!assetBeenSetup)
        {
            SetupAsset();
            assetBeenSetup = true;
        }

        if (stateMachine != null)
        {
            stateMachine.ChangeState(DownState);
        }
        if (Corrupted)
        {
            GameEvents.OnMLCardSetToCorrupt.Publish(this);
        }
    }

    private void SetupAsset()
    {
        if (MLCardThemeManager.Instance == null)
        {
            Debug.LogWarning("MLCardThemeManager is null");
            return;
        }

        if (icon != null)
        {
            icon.sprite = MLCardThemeManager.Instance.GetCardIcon(CardNumber);
        }
        if (nameDisplay != null)
        {
            nameDisplay.SetText(MLCardThemeManager.Instance.GetCardName(CardNumber));
        }
    }

    public void SetReveal(bool revealed)
    {
        if (icon != null)
        {
            icon.enabled = revealed;
        }
        if (nameDisplay != null)
        {
            nameDisplay.enabled = revealed;
        }
    }

    public void SetBackgroundColor(Color color)
    {
        if (background != null)
        {
            background.color = color;
        }
    }

    private Coroutine flipAnimation;
    public void StartFlipAnimation(bool isUp)
    {
        IEnumerator FlipAnimation()
        {
            float animDur = 1f;
            float animTime = 0f;

            while (animTime < animDur)
            {
                yield return new WaitForEndOfFrame();
                animTime += Time.unscaledDeltaTime;

                if (background != null)
                {
                    background.transform.localRotation =
                        Quaternion.Euler(
                            0f,
                            Mathf.Lerp(background.transform.localRotation.eulerAngles.y, isUp ? 0f : 180f, Time.unscaledDeltaTime * 10f),
                            0f);
                }
            }
        }
        if (flipAnimation != null)
            StopCoroutine(flipAnimation);
        flipAnimation = StartCoroutine(FlipAnimation());
    }

    public void EmitParticles()
    {
        if (psExplode != null)
        {
            var main = psExplode.main;
            main.startColor = Corrupted ? Color.red : Color.green;

            psExplode.Emit(16);
        }
    }

    private void EmitFailPairParticles(CardPairArgument arg)
    {
        if (arg.card1.Equals(this) || arg.card2.Equals(this))
        {
            if (psExplode != null)
            {
                var main = psExplode.main;
                main.startColor = Color.red;
                psExplode.Emit(16);
            }
        }
    }
}
