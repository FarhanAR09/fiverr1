using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RAMGrid : MonoBehaviour
{
    public static RAMGrid Instance { get; private set; }

    [SerializeField]
    private List<RAMStick> sticks;

    //[SerializeField]
    private int row = 3, column = 4;

    public int Row { get { return row; } }
    public int Column { get { return column; } }

    private bool beenSetup = false;

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

    public void Setup(int level)
    {
        if (beenSetup) return;
        beenSetup = true;

        switch (level)
        {
            case 1:
                row = 4;
                column = 4;
                break;
            case 2:
                row = 4;
                column = 5;
                break;
            case 3:
                row = 5;
                column = 6;
                break;
            case 4:
                row = 6;
                column = 6;
                break;
            case 5:
                row = 6;
                column = 7;
                break;
            default:
                row = 4;
                column = 4;
                break;
        }

        //Setting difficulty
        List<int> activeSticksIndexes = GetRAMStickActiveIndex(column);
        for (int i = 0; i < sticks.Count; i++)
        {
            if (!activeSticksIndexes.Contains(i))
            {
                Destroy(sticks[i].gameObject);
            }
        }

        //Count cards
        int cardCount = row * column;
        //foreach (RAMStick stick in sticks)
        //{
        //    if (stick.enabled)
        //        cardCount += stick.GetStickCardCount();
        //}

        //Generate card
        List<int>  cardIds = new();
        for (int i = 0; i < cardCount / 2; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        //Shuffle card
        Shuffle(cardIds);

        //Assign card number to cards
        int assignedCount = 0;
        for (int i = 0; i < sticks.Count; i++)
        {
            if (!activeSticksIndexes.Contains(i))
                continue;
            sticks[i].Setup(cardIds.GetRange(assignedCount, row));
            assignedCount += row;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }

    private List<int> GetRAMStickActiveIndex(int activeStickNumber)
    {
        return activeStickNumber switch
        {
            1 => new List<int> { 3 },
            2 => new List<int> { 2, 3 },
            3 => new List<int> { 2, 3, 4 },
            4 => new List<int> { 1, 2, 3, 4 },
            5 => new List<int> { 1, 2, 3, 4, 5 },
            6 => new List<int> { 1, 2, 3, 4, 5, 6 },
            7 => new List<int> { 0, 1, 2, 3, 4, 5, 6 },
            _ => new List<int> { 2, 3},
        };
    }
}
