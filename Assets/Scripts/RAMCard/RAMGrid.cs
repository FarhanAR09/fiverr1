using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class RAMGrid : MonoBehaviour
{
    public static RAMGrid Instance { get; private set; }

    [SerializeField]
    private List<RAMStick> sticks;

    //[SerializeField]
    private int row = 3, column = 4;

    public int Row { get { return row; } }
    public int Column { get { return column; } }

    public UnityAction onGridSetup;

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

    public void SetupByLevel(int level)
    {
        if (MLMainMenuFeatureSwitches.DifficultyOverridden)
        {
            SetupBySize(MLMainMenuFeatureSwitches.DebugColumnCount, MLMainMenuFeatureSwitches.DebugRowCount);
            return;
        }
        int col, row;
        switch (level)
        {
            case 1:
                row = 4;
                col = 4;
                break;
            case 2:
                row = 4;
                col = 5;
                break;
            case 3:
                row = 5;
                col = 6;
                break;
            case 4:
                row = 6;
                col = 6;
                break;
            case 5:
                row = 6;
                col = 7;
                break;
            default:
                row = 4;
                col = 4;
                break;
        }
        SetupBySize(col, row);
    }

    public void SetupBySize(int c, int r)
    {
        column = Mathf.Clamp(c, 1, sticks.Count);
        row = Mathf.Clamp(r, 1, 6);
        if (column * row % 2 != 0)
        {
            row = 6;
        }

        //Setting Active Column
        List<int> activeSticksIndexes = GetRAMStickActiveIndex(column);
        for (int i = 0; i < sticks.Count; i++)
        {
            if (!activeSticksIndexes.Contains(i))
            {
                sticks[i].DisableStick();
            }
            //sticks[i].gameObject.SetActive(isActive);
        }

        int cardCount = row * column;

        //Generate card numbers
        List<int> cardIds = new();
        for (int i = 0; i < cardCount / 2; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        //Shuffle card
        Shuffle(cardIds);

        //Generate corrupted card numbers
        //Corrupted numbers = 20% of pairs starting from number 0, 1, ...
        List<int> corruptedNumbers = new();
        for (int i = 0; i < Mathf.FloorToInt(cardCount * 0.2f / 2f); i++)
        {
            corruptedNumbers.Add(i);
        }

        //Assign card number to cards
        int assignedCount = 0;
        for (int i = 0; i < sticks.Count; i++)
        {
            if (!activeSticksIndexes.Contains(i))
                continue;
            sticks[i].Setup(cardIds.GetRange(assignedCount, row), corruptedNumbers);
            assignedCount += row;
        }

        onGridSetup.Invoke();
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
