using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerPowerUpManager))]
public class PlayerAnimation : MonoBehaviour
{
    private PlayerInput input;
    [SerializeField]
    private Transform spriteTransform;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private PlayerPowerUpManager powerManager;
    [SerializeField]
    private ParticleSystem trail;
    private ParticleSystem.MainModule trailMain;

    [SerializeField]
    private ParticleSystem psExplode;
    private ParticleSystem.MainModule psExplodeMain;

    [SerializeField]
    private AudioClip playerDeathSFX, playerEmpSFX, playerSlowSFX;

    [SerializeField]
    private TrailRenderer boostTrailRenderer;
    [SerializeField]
    private ParticleSystem psBoostTrail, psBoostExplode;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        if (trail != null)
        {
            trailMain = trail.main;
        }
        else Debug.Log("Trail particle system is null");

        powerManager = GetComponent<PlayerPowerUpManager>();

        if (psExplode != null)
        {
            psExplodeMain = psExplode.main;
        }
    }

    private void Start()
    {
        if (boostTrailRenderer != null)
        {
            boostTrailRenderer.emitting = false;
        }
        if (psBoostTrail != null)
        {
            psBoostTrail.Stop();
        }
    }

    private void OnEnable()
    {
        powerManager.OnBulletTimeActivated.AddListener(Explode);
        powerManager.OnEMPThrown.AddListener(EmpSFX);
        GameEvents.OnPlayerLose.Add(DeathSFX);

        powerManager.OnBoostStart.AddListener(StartBoostEffects);
        powerManager.OnBoostEnd.AddListener(EndBoostEffects);
    }

    private void Update()
    {
        //Sprite rotation
        if (spriteTransform != null)
        {
            spriteTransform.localScale = new(1, 1, 1);
            spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            switch (input.StoredDirection)
            {
                case MovementDirection.Up:
                    spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case MovementDirection.Down:
                    spriteTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                    break;
                case MovementDirection.Left:
                    spriteTransform.localScale = new(-1, 1, 1);
                    break;
                case MovementDirection.Right:
                    spriteTransform.localScale = new(1, 1, 1);
                    break;
            }
        }

        if (spriteRenderer)
        {
            Color playerGlowColor = powerManager.AvailableCharge switch
            {
                0 => Color.red,
                1 => Color.yellow,
                2 => Color.green,
                3 => Color.cyan,
                _ => Color.red
            };
            spriteRenderer.material.SetColor("_Color", playerGlowColor);
        }

        if (trail != null)
        {
            trailMain.startColor = powerManager.AvailableCharge switch
            {
                0 => (ParticleSystem.MinMaxGradient)Color.red,
                1 => (ParticleSystem.MinMaxGradient)Color.yellow,
                2 => (ParticleSystem.MinMaxGradient)Color.green,
                3 => (ParticleSystem.MinMaxGradient)Color.cyan,
                _ => (ParticleSystem.MinMaxGradient)Color.red,
            };
        }
    }

    private void OnDisable()
    {
        powerManager.OnBulletTimeActivated.RemoveListener(Explode);
        powerManager.OnEMPThrown.RemoveListener(EmpSFX);
        GameEvents.OnPlayerLose.Remove(DeathSFX);

        powerManager.OnBoostStart.RemoveListener(StartBoostEffects);
        powerManager.OnBoostEnd.RemoveListener(EndBoostEffects);
    }

    private void Explode()
    {
        if (psExplode != null)
        {
            psExplodeMain.startColor = (powerManager.AvailableCharge + 1) switch
            {
                1 => Color.yellow,
                2 => Color.green,
                3 => Color.cyan,
                _ => Color.red
            };
            psExplode.Emit(60);
        }

        if (playerSlowSFX != null && SFXController.Instance != null)
        {
            SFXController.Instance.RequestPlay(playerSlowSFX, 19999);
        }
    }

    private void DeathSFX(bool s)
    {
        if (playerDeathSFX != null && SFXController.Instance != null)
        {
            SFXController.Instance.RequestPlay(playerDeathSFX, 20000, timePitching: false);
        }
    }

    private void EmpSFX()
    {
        if (playerEmpSFX != null && SFXController.Instance != null)
        {
            SFXController.Instance.RequestPlay(playerEmpSFX, 19999);
        }
    }

    private void StartBoostEffects()
    {
        if (boostTrailRenderer != null)
        {
            boostTrailRenderer.emitting = true;
        }
        if (psBoostTrail != null)
        {
            psBoostTrail.Play();
        }
        if (psBoostExplode != null)
        {
            psBoostExplode.Emit(20);
        }
    }

    private void EndBoostEffects()
    {
        if (boostTrailRenderer != null)
        {
            boostTrailRenderer.emitting = false;
        }
        if (psBoostTrail != null)
        {
            psBoostTrail.Stop();
        }
        if (psBoostExplode != null)
        {
            psBoostExplode.Emit(20);
        }
    }
}