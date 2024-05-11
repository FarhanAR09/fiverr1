using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Battery : MonoBehaviour, IStunnable
{
    public UnityEvent OnBatteryFull { get; private set; } = new();
    private bool previouslyNotFull = true;

    /// <summary>
    /// Normalized charge
    /// </summary>
    public float CurrentCharge { get; private set; } = 0;

    private bool hasCooldown = true;
    private const float instantChargeDuration = 0.5f;
    private float ChargeDuration
    {
        get
        {
            if (hasCooldown)
                return 10f;
            else
                return instantChargeDuration;
        }
    }

    [SerializeField]
    private List<Sprite> stateSprites = new ();
    private int spriteStatesNum;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private ParticleSystem psDischarge;

    public bool AllowCharging { get; set; } = false;

    private void Awake()
    {
        spriteStatesNum = stateSprites.Count;
    }

    private void Start()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material.SetFloat("_Intensity", 0.75f);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSwitchCooldownCharge.Add(HasCooldownState);
    }

    private void OnDisable()
    {
        GameEvents.OnSwitchCooldownCharge.Remove(HasCooldownState);
    }

    private void FixedUpdate()
    {
        if (AllowCharging)
        {
            if (CurrentCharge < 1f)
            {
                CurrentCharge += Time.fixedDeltaTime / ChargeDuration;
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
        }

        if (spriteRenderer != null)
        {
            int index = GetIndexByCharge();
            spriteRenderer.sprite = stateSprites[index];
            spriteRenderer.material.SetColor("_Color", GetChargeColor(index));
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

    private int GetIndexByCharge()
    {
        return Mathf.Min(spriteStatesNum - 1, Mathf.FloorToInt((spriteStatesNum - 1) * CurrentCharge));
    }

    private Color GetChargeColor()
        => GetChargeColor(GetIndexByCharge());

    private Color GetChargeColor(int index)
    {
        return index switch
        {
            0 => Color.red,
            1 => Color.red,
            2 => Color.yellow,
            3 => Color.green,
            4 => Color.cyan,
            _ => Color.red
        };
    }

    public void Stun(float duration)
    {
        if (psDischarge != null)
        {
            var main = psDischarge.main;
            main.startColor = GetChargeColor();
            psDischarge.GetComponent<ParticleSystemRenderer>().material.SetColor("_GlowColor", GetChargeColor());
            psDischarge.Emit(15);
        }
        CurrentCharge = 0f;
    }

    private void HasCooldownState(bool state)
    {
        hasCooldown = state;
    }
}
