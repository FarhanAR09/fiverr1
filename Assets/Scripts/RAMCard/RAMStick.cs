using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAMStick : MonoBehaviour
{
    [SerializeField]
    private List<RAMCard> cards;

    public void Setup(List<int> cardNumbers, List<int> corruptedNumbers)
    {
        int allCardsCount = cards.Count;

        List<int> activeIndexes = GetRAMCardActiveIndex(cardNumbers.Count);
        int assignedIndex = 0;
        for (int i = 0; i < allCardsCount; i++)
        {
            if (activeIndexes.Contains(i))
            {
                cards[i].gameObject.SetActive(true);
                cards[i].Setup(
                    cardNumbers[assignedIndex],
                    corruptedNumbers != null && corruptedNumbers.Contains(cardNumbers[assignedIndex]));
                assignedIndex++;
            }
            else
            {
                cards[i].gameObject.SetActive(false);
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
