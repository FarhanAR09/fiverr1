using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Battery : MonoBehaviour
{
    public UnityEvent OnBatteryFull { get; private set; } = new();
    private bool previouslyNotFull = true;

    /// <summary>
    /// Normalized charge
    /// </summary>
    public float CurrentCharge { get; private set; } = 0;

    private const float chargeDuration = 10f;

    [SerializeField]
    private List<Sprite> stateSprites = new ();
    private int spriteStatesNum;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteStatesNum = stateSprites.Count;
    }

    private void FixedUpdate()
    {
        if (CurrentCharge < 1f)
        {
            CurrentCharge += Time.fixedDeltaTime / chargeDuration;
            previouslyNotFull = true;
        }
        else
        {
            if (previouslyNotFull)
            {
                OnBatteryFull.Invoke();
                previouslyNotFull = false;
                //Debug.Log(name + " is full!");
            }
            CurrentCharge = 1f;
        }

        if (spriteRenderer != null)
        {
            int index = Mathf.Min(spriteStatesNum - 1, Mathf.FloorToInt(spriteStatesNum * CurrentCharge));
            spriteRenderer.sprite = stateSprites[index];
        }
    }

    public bool TryTakeCharge()
    {
        if (CurrentCharge >= 1f)
        {
            CurrentCharge = 0;
            return true;
        }
        return false;
    }
}
