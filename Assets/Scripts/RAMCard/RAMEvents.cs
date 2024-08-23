using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class GameEvents
{
    public static GameEvent<CardFlipArgument> OnMLCardFlipped = new();
    public static GameEvent<CardPairArgument> OnMLCardsPaired = new();
    public static GameEvent<CardPairArgument> OnMLCorruptCardsPaired = new();
    public static GameEvent<CardPairArgument> OnMLCardsFailPairing = new();
    public static GameEvent<RAMCard> OnMLCardExitUpState = new();
    public static GameEvent<RAMCard> OnMLCardFinishedSingleCheck = new();
    public static GameEvent<RAMCard> OnMLCardSetToCorrupt = new();
    public static GameEvent<bool> OnMLAllCardsPaired = new();
}

public class CardFlipArgument
{
    public RAMCard card;
    public bool isUp;

    public CardFlipArgument(RAMCard card, bool isUp)
    {
        this.card = card;
        this.isUp = isUp;
    }
}

public class CardPairArgument
{
    public RAMCard card1, card2;

    public CardPairArgument(RAMCard card1, RAMCard card2)
    {
        this.card1 = card1;
        this.card2 = card2;
    }
}

public class CardChangeStateArgument
{
    public RAMCard card;
    public State newState;

    public CardChangeStateArgument(RAMCard card, State newState)
    {
        this.card = card;
        this.newState = newState;
    }
}
