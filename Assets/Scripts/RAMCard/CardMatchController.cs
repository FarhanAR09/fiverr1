using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CardMatchController : MonoBehaviour
{
    public static CardMatchController Instance { get; private set; }

    private List<RAMCard> openedCards = new();

    [SerializeField]
    private RAMGrid ramGrid;
    public int MaxPairCount { get; private set; } = 0;
    public int PairCount { get; private set; } = 0;
    public int TimesCardOpened { get; private set; } = 0;


    private void OnEnable()
    {
        GameEvents.OnMLCardFlipped.Add(CheckCard);
        GameEvents.OnMLCardsPaired.Add(Pairing);
        GameEvents.OnMLCardExitUpState.Add(RemoveCardFromList);
        GameEvents.OnMLCardSetToCorrupt.Add(ReduceMaxPairCount);

        GameEvents.OnMLCorruptCardsPaired.Add(CorruptPairing);
    }

    private void OnDisable()
    {
        GameEvents.OnMLCardFlipped.Remove(CheckCard);
        GameEvents.OnMLCardsPaired.Remove(Pairing);
        GameEvents.OnMLCardExitUpState.Remove(RemoveCardFromList);
        GameEvents.OnMLCardSetToCorrupt.Remove(ReduceMaxPairCount);

        GameEvents.OnMLCorruptCardsPaired.Remove(CorruptPairing);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //FIX: this should wait until grid finish setting up, but it's fine now
        IEnumerator Wait()
        {
            yield return new WaitUntil(() => ramGrid != null && ramGrid.BeenSetup);
            if (ramGrid != null)
            {
                MaxPairCount = Mathf.FloorToInt(ramGrid.Row * ramGrid.Column / 2f);
                MaxPairCount -= reducePairCount;
            }
        }
        StartCoroutine(Wait());
    }

    private void CheckCard(CardFlipArgument arg)
    {
        if (arg.card == null)
            return;
        
        if (arg.isUp)
        {
            TimesCardOpened++;
            //print("Adding card " + arg.card.name);
            openedCards.Add(arg.card);
            //print(openedCards.Count);
            //Delayed to give time for state initialization
            IEnumerator DelayAFrame()
            {
                yield return new WaitForFixedUpdate();
                if (openedCards.Count >= 2)
                {
                    if (openedCards[0].CardNumber == openedCards[1].CardNumber)
                    {
                        //print("Card count: " + openedCards.Count);
                        RAMCard card1 = openedCards[0];
                        RAMCard card2 = openedCards[1];
                        card1.PairCard();
                        card2.PairCard();
                        if (card1.Corrupted)
                        {
                            if (MLScoreManager.Instance != null)
                                MLScoreManager.Instance.AddScore(-25f);
                            GameEvents.OnMLCorruptCardsPaired.Publish(new CardPairArgument(card1, card2));
                        }
                        else
                        {
                            if (MLScoreManager.Instance != null)
                                MLScoreManager.Instance.AddScore(10f);
                            GameEvents.OnMLCardsPaired.Publish(new CardPairArgument(card1, card2));
                        }
                    }
                    else
                    {
                        //Debug.Log("Bruh u stupid");
                        RAMCard card1 = openedCards[0];
                        RAMCard card2 = openedCards[1];
                        IEnumerator DelayPutDown()
                        {
                            yield return new WaitForSeconds(0.5f);
                            card1.PutDownCard();
                            card2.PutDownCard();
                            GameEvents.OnMLCardsFailPairing.Publish(new(card1, card2));
                            if (MLScoreManager.Instance != null)
                                MLScoreManager.Instance.AddScore(-10f);
                        }
                        StartCoroutine(DelayPutDown());
                    }
                    //Already cleared by HandleCardNotUpAnymore, just a guard
                    openedCards.Clear();
                }
            }
            StartCoroutine(DelayAFrame());
        }
    }

    private void RemoveCardFromList(RAMCard card)
    {
        if (openedCards.Contains(card))
        {
            openedCards.Remove(card);
        }
    }

    private void Pairing(CardPairArgument arg)
    {
        PairCount++;
        print("Pairing: " + arg.card1.name + " " + arg.card2.name);
        if (PairCount >= MaxPairCount)
        {
            print("All paired!");
            GameEvents.OnMLAllCardsPaired.Publish(true);
        }
    }

    private void CorruptPairing(CardPairArgument arg)
    {
        print("Corrupt Pairing: " + arg.card1.name + " " + arg.card2.name);
    }

    private int reduceCardCount = 0, reducePairCount = 0;
    private void ReduceMaxPairCount(RAMCard _)
    {
        reduceCardCount++;
        if (reduceCardCount >= 2)
        {
            reducePairCount++;
            reduceCardCount = 0;
        }
    }
}
