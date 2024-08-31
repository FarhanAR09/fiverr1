using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMatchController : MonoBehaviour
{
    public static CardMatchController Instance { get; private set; }

    private List<RAMCard> openedCards = new();

    public int MaxPairCount { get; private set; } = 0;
    public int PairCount { get; private set; } = 0;
    public int TimesCardOpened { get; private set; } = 0;

    private bool gridIsSetup = false;

    private void OnEnable()
    {
        GameEvents.OnMLCardFlipped.Add(CheckCard);
        GameEvents.OnMLCardsPaired.Add(Pairing);
        GameEvents.OnMLCardExitUpState.Add(RemoveCardFromList);
        GameEvents.OnMLCardSetToCorrupt.Add(ReduceMaxPairCount);

        GameEvents.OnMLAllCardsPaired.Add(ResetPairCount);

        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.onGridSetup += TrackGridSetupState;
        }
    }

    private void OnDisable()
    {
        GameEvents.OnMLCardFlipped.Remove(CheckCard);
        GameEvents.OnMLCardsPaired.Remove(Pairing);
        GameEvents.OnMLCardExitUpState.Remove(RemoveCardFromList);
        GameEvents.OnMLCardSetToCorrupt.Remove(ReduceMaxPairCount);

        GameEvents.OnMLAllCardsPaired.Remove(ResetPairCount);

        if (RAMGrid.Instance != null)
        {
            RAMGrid.Instance.onGridSetup -= TrackGridSetupState;
        }
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
        IEnumerator Wait()
        {
            yield return new WaitUntil(() => RAMGrid.Instance != null && gridIsSetup);
            
            gridIsSetup = false;
            if (RAMGrid.Instance != null)
            {
                MaxPairCount = Mathf.FloorToInt(RAMGrid.Instance.Row * RAMGrid.Instance.Column / 2f);
                MaxPairCount -= reducePairCount;
            }
        }
        StopCoroutine(Wait());
        StartCoroutine(Wait());
    }

    private void CheckCard(CardFlipArgument arg)
    {
        if (arg.card == null)
            return;
        
        if (arg.isUp)
        {
            TimesCardOpened++;
            openedCards.Add(arg.card);

            //Delayed to give time for state initialization
            IEnumerator DelayAFrame()
            {
                if (openedCards.Count >= 2)
                {
                    RAMCard card1 = openedCards[0];
                    RAMCard card2 = openedCards[1];
                    openedCards.Clear();
                    yield return new WaitForFixedUpdate();
                    if (card1.CardNumber == card2.CardNumber)
                    {
                        card1.PairCard();
                        card2.PairCard();
                        if (card1.Corrupted || card2.Corrupted)
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
                        card1.ViewFailedPairCard();
                        card2.ViewFailedPairCard();
                        yield return new WaitForSeconds(0.5f);
                        card1.PutDownCard();
                        card2.PutDownCard();
                        GameEvents.OnMLCardsFailPairing.Publish(new(card1, card2));
                        if (card1.Corrupted || card2.Corrupted)
                            GameEvents.OnMLCorruptCardsPaired.Publish(new CardPairArgument(card1, card2));
                        if (MLScoreManager.Instance != null)
                            MLScoreManager.Instance.AddScore(-10f);
                    }
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
            if (MLPlayManager.Instance.GameMode == MLGameMode.Classic)
            {
                GameEvents.OnMLGameFinished.Publish(true);
            }
            GameEvents.OnMLAllCardsPaired.Publish(true);
        }
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

    private void TrackGridSetupState()
    {
        gridIsSetup = true;
    }

    private void ResetPairCount(bool _)
    {
        PairCount = 0;
    }
}
