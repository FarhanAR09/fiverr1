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

    public bool IsOpen { get; private set; } = false;
    public bool IsDisabled { get; private set; } = false;

    public UnityAction OnClicked, PairRequestCalled, PutDownRequestCalled;


    private StateMachine stateMachine;
    public CardDownState DownState { get; private set; }
    public CardUpState UpState { get; private set; }
    public CardPairedState PairedState { get; private set; }
    public CardPeekedState PeekedState { get; private set; }

    [SerializeField]
    private TMP_Text display;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        TryGetComponent(out stateMachine);
        if (stateMachine != null)
        {
            DownState = new(this, stateMachine);
            UpState = new(this, stateMachine);
            PairedState = new(this, stateMachine);
            PeekedState = new(this, stateMachine);

            stateMachine.ChangeState(DownState);
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

    public void Setup(int cardNumber)
    {
        //print($"{name} was setup with {cardNumber}");
        CardNumber = cardNumber;
        
        //TODO: setup visuals
        if (display != null)
        {
            display.SetText(cardNumber.ToString());
            display.enabled = false;
        }
    }

    public void NumberRevealed(bool revealed)
    {
        if (display != null)
        {
            display.enabled = revealed;
        }
    }
}
