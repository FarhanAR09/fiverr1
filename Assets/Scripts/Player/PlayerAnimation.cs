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

    private const string HURT = "hurtTimeFreeze";

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
        powerManager.OnEMPThrown.AddListener(PlayerDisappear);
        GameEvents.OnPlayerLose.Add(DeathSFX);

        powerManager.OnBoostStart.AddListener(StartBoostEffects);
        powerManager.OnBoostEnd.AddListener(EndBoostEffects);
        
        GameEvents.OnPlayerHurt.Add(StartHurtAnimation);
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

        //if (assemblingAnim)
        //{
        //    if (assembleTime < assembleDur)
        //    {
        //        Debug.Log(assembleTime);
        //        assembleTime += Time.deltaTime;
        //        float norm = assembleTime / assembleDur;

        //        if (assembledSprite != null)
        //        {
        //            assembledSprite.color = new(1f, 1f, 1f, Mathf.Min(norm, 1f));
        //            assembledSprite.transform.localScale = ((1 - norm) * 9f + 1f) * Vector3.one;
        //        }
        //    }
        //    else
        //    {
        //        assemblingAnim = false;
        //    }
        //}
    }

    private void OnDisable()
    {
        powerManager.OnBulletTimeActivated.RemoveListener(Explode);
        powerManager.OnEMPThrown.RemoveListener(EmpSFX);
        powerManager.OnEMPThrown.RemoveListener(PlayerDisappear);
        GameEvents.OnPlayerLose.Remove(DeathSFX);

        powerManager.OnBoostStart.RemoveListener(StartBoostEffects);
        powerManager.OnBoostEnd.RemoveListener(EndBoostEffects);
        
        GameEvents.OnPlayerHurt.Remove(StartHurtAnimation);
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

    private void DeathSFX(bool _)
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

    private void PlayerDisappear()
    {
        IEnumerator Timing()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(0.5f);
                spriteRenderer.enabled = true;
            }
        }
        StopCoroutine(Timing());
        StartCoroutine(Timing());
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

    private void StartHurtAnimation(bool _)
    {
        IEnumerator HurtAnimation()
        {
            //Debug.Log("Started");

            if (!GameSpeedManager.TryModifyGameSpeedModifier(HURT, 0f))
                GameSpeedManager.TryAddGameSpeedModifier(HURT, 0f);

            SpriteRenderer assembledSprite = null;
            if (spriteTransform != null)
            {
                Vector2 mapCenter = Vector2.zero;
                if (MapHandler.Instance != null && MapHandler.Instance.MapGrid != null)
                {
                    float cellSize = MapHandler.Instance.MapGrid.GetCellSize();
                    mapCenter = new Vector2(
                        Mathf.Lerp(0f, cellSize * MapHandler.Instance.MapGrid.GetWidth(), 0.5f),
                        Mathf.Lerp(0f, cellSize * MapHandler.Instance.MapGrid.GetHeight(), 0.5f));
                }

                assembledSprite = Instantiate(spriteTransform.gameObject, mapCenter, new Quaternion(), transform).GetComponent<SpriteRenderer>();
                assembledSprite.color = new(1f, 1f, 1f, 0f);
                assembledSprite.transform.localScale = new(10f, 10f, 10f);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            //Assemble
            float assembleDur = 0.5f, assembleTime = 0f;
            do
            {
                yield return new WaitForEndOfFrame();
                assembleTime += Time.unscaledDeltaTime;
                float t = assembleTime / assembleDur;

                if (assembledSprite != null)
                {
                    assembledSprite.color = new Color(1f, 1f, 1f, t * t * (3f - 2f * t));
                    assembledSprite.transform.localScale = Mathf.Lerp(32f, 10f, t * t * (3f - 2f * t)) * Vector3.one;
                }
            }
            while (assembleTime < assembleDur);

            yield return new WaitForSecondsRealtime(0.2f);

            //Move
            float moveDur = 1f, moveTime = 0f;
            Vector3 initialPosition = assembledSprite != null ? assembledSprite.transform.position : Vector3.zero;
            do
            {
                yield return new WaitForEndOfFrame();
                moveTime += Time.unscaledDeltaTime;
                float t = moveTime / moveDur;
                t = t * t * (3f - 2f * t);

                if (assembledSprite != null)
                {
                    assembledSprite.transform.position = Vector3.Lerp(initialPosition, transform.position, t);
                    assembledSprite.transform.localScale = Mathf.Lerp(10f, 1f, t) * Vector3.one;
                }
            }
            while (moveTime < moveDur);

            yield return new WaitForSecondsRealtime(0.3f);

            if (assembledSprite != null)
            {
                Destroy(assembledSprite.gameObject);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
            }

            //yield return new WaitForSecondsRealtime(0.5f);
            //yield return new WaitForSecondsRealtime(3f);

            if (!GameSpeedManager.TryModifyGameSpeedModifier(HURT, 1f))
                GameSpeedManager.TryAddGameSpeedModifier(HURT, 1f);
        }
        StopCoroutine(HurtAnimation());
        if (PlayerInput.Instance != null && PlayerInput.Lives > 0)
            StartCoroutine(HurtAnimation());
    }
}