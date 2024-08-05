using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAMStick : MonoBehaviour
{
    [SerializeField]
    private List<RAMCard> cards;

    public void Setup(List<int> cardNumbers)
    {
        //if (cards.Count != cardNumbers.Count)
        //{
        //    Debug.LogError("CardNumbers amount mismatches with number of cards in " + name);
        //    return;
        //}

        int allCardsCount = cards.Count;

        //for (int i = activeCardsCount; i < allCardsCount; i++)
        //{
        //    Destroy(cards[i].gameObject);
        //}

        //for (int i = 0; i < activeCardsCount; i++)
        //{
        //    cards[i].Setup(cardNumbers[i]);
        //}

        List<int> activeIndexes = GetRAMCardActiveIndex(cardNumbers.Count);
        int assignedIndex = 0;
        for (int i = 0; i < allCardsCount; i++)
        {
            if (activeIndexes.Contains(i))
            {
                cards[i].Setup(cardNumbers[assignedIndex]);
                assignedIndex++;
            }
            else
            {
                Destroy(cards[i].gameObject);
            }
        }
    }

    public int GetStickCardCount()
    {
        return cards.Count;
    }

    private List<int> GetRAMCardActiveIndex(int activeCardNumber)
    {
        return activeCardNumber switch
        {
            1 => new List<int> { 2 },
            2 => new List<int> { 2, 3 },
            3 => new List<int> { 1, 2, 3, },
            4 => new List<int> { 1, 2, 3, 4 },
            5 => new List<int> { 0, 1, 2, 3, 4 },
            6 => new List<int> { 0, 1, 2, 3, 4, 5 },
            _ => new List<int> { 2, 3 },
        };
    }
}
