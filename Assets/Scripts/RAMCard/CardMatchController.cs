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
        GameEvents.OnCardFlipped.Add(CheckCard);
        GameEvents.OnCardsPaired.Add(Pairing);
        GameEvents.OnCardExitUpState.Add(RemoveCardFromList);
    }

    private void OnDisable()
    {
        GameEvents.OnCardFlipped.Remove(CheckCard);
        GameEvents.OnCardsPaired.Remove(Pairing);
        GameEvents.OnCardExitUpState.Remove(RemoveCardFromList);
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
        if (ramGrid != null)
        {
            MaxPairCount = Mathf.FloorToInt(ramGrid.Row * ramGrid.Column / 2f);
        }
    }

    private void CheckCard(CardFlipArgument arg)
    {
        //if (flippedCard != null)
        //{
        //    if (flippedCard.IsOpen)
        //    {
        //        if (!selected1)
        //        {
        //            Debug.Log("Card1 filled");
        //            card1 = flippedCard;
        //            card2 = null;
        //        }
        //        else if (!selected2 && !flippedCard.Equals(card1))
        //        {
        //            Debug.Log("Card2 filled");
        //            card2 = flippedCard;

        //            Debug.Log("Checking");
        //            if (card1.CardNumber == card2.CardNumber)
        //                Debug.Log("Match");
        //            else
        //                Debug.Log("Bruh u stupid");

        //            //TODO: disable card

        //            card1 = null;
        //            card2 = null;
        //        }
        //    }
        //    else
        //    {
        //        flippedCard.CloseCard();
        //    }
        //}
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
                        GameEvents.OnCardsPaired.Publish(new CardPairArgument(card1, card2));
                        if (MLScoreManager.Instance != null)
                            MLScoreManager.Instance.AddScore(10f);
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
                            GameEvents.OnCardsFailPairing.Publish(new(card1, card2));
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
        if (PairCount >= MaxPairCount)
        {
            print("All paired!");
            GameEvents.OnAllCardsPaired.Publish(true);
        }
    }
}
