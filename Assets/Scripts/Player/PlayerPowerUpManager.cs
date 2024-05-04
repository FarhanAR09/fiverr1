using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PlayerPowerUpManager : MonoBehaviour
{
    private const string BULLETTIME = "BulletTimeSpeed";
    private Coroutine bulletTimeCoroutine;
    private bool bulletTimeActive = false;
    
    private PlayerInput playerInput;

    [SerializeField]
    private GameObject empControllerPrefab;

    public int MaxCharge { get; private set; } = 3;
    public int AvailableCharge { get; private set; } = 0;

    public UnityEvent OnBulletTimeActivated { get; private set; } = new();
    public UnityEvent OnBulletTimeDeactivated { get; private set; } = new();
    public UnityEvent OnEMPThrown { get; private set; } = new();

    [SerializeField]
    private AudioClip fillChargeSFX;

    //Boost
    private bool boostStarted;
    private float boostTime = 0f;
    private readonly float boostDuration = 3f;
    public UnityEvent OnBoostStart { get; private set; } = new();
    public UnityEvent OnBoostEnd { get; private set; } = new();
    [SerializeField]
    private GameObject purgingTrailPrefab;
    private float layPurgeTime = 0f;
    private readonly float layPurgeCooldown = 0.08f;
    [SerializeField]
    private GameObject purgingPlayerBodyPrefab;
    private GameObject purgingPlayerBody;
    [SerializeField]
    private AudioClip boostStartSfx, boostEndSfx;

    private void Awake()
    {
        if (TryGetComponent(out PlayerInput _playerInput))
        {
            playerInput = _playerInput;
        }
    }

    private void Start()
    {
        GameSpeedManager.TryAddGameSpeedModifier(BULLETTIME, 1f);
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.OnSpaceDown.AddListener(BulletTime);
            playerInput.OnVDown.AddListener(ThrowEMP);

            playerInput.OnBoostDown.AddListener(StartBoost);
            playerInput.OnBoostUp.AddListener(EndBoost);
            playerInput.OnHitWall.AddListener(EndBoost);
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.OnSpaceDown.RemoveListener(BulletTime);
            playerInput.OnVDown.RemoveListener(ThrowEMP);

            playerInput.OnBoostDown.RemoveListener(StartBoost);
            playerInput.OnBoostUp.RemoveListener(EndBoost);
            playerInput.OnHitWall.RemoveListener(EndBoost);
        }

        if (bulletTimeCoroutine != null)
        {
            StopCoroutine(bulletTimeCoroutine);
            GameSpeedManager.TryModifyGameSpeedModifier(BULLETTIME, 1);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        OnBulletTimeActivated.RemoveAllListeners();
        OnBulletTimeDeactivated.RemoveAllListeners();
        OnEMPThrown.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
     //Lay Purge   
        if (boostStarted)
        {
            if (layPurgeTime > 0)
            {
                layPurgeTime -= Time.fixedDeltaTime;
            }
            else
            {
                layPurgeTime = layPurgeCooldown;

                //Lay Purge Trail
                if (purgingTrailPrefab != null)
                {
                    Destroy(Instantiate(purgingTrailPrefab, transform.position, new Quaternion()), 0.5f);
                }
            }
        }
    }

    public void FillCharge(int amount)
    {
        if (amount > 0 && fillChargeSFX != null && SFXController.Instance != null)
        {
            SFXController.Instance.RequestPlay(fillChargeSFX, 15000);
        }
        AvailableCharge += amount;
    }
    
    private void BulletTime()
    {
        void Deactivate()
        {
            bulletTimeActive = false;
            if (bulletTimeCoroutine != null)
                StopCoroutine(bulletTimeCoroutine);
            GameSpeedManager.TryModifyGameSpeedModifier(BULLETTIME, 1);
            
            OnBulletTimeDeactivated.Invoke();
        }

        int usedCharge = 1;
        if (AvailableCharge >= usedCharge)
        {
            if (!bulletTimeActive)
            {
                AvailableCharge -= usedCharge;
                bulletTimeActive = true;
                IEnumerator BulletTime()
                {
                    OnBulletTimeActivated.Invoke();
                    float calculatedBulletTimeSpeed = 1;
                    while (true)
                    {
                        calculatedBulletTimeSpeed = Mathf.Lerp(calculatedBulletTimeSpeed, 0.1f, 0.5f);
                        GameSpeedManager.TryModifyGameSpeedModifier(BULLETTIME, calculatedBulletTimeSpeed);

                        yield return new WaitForFixedUpdate();
                    }
                }
                if (bulletTimeCoroutine != null)
                    StopCoroutine(bulletTimeCoroutine);
                bulletTimeCoroutine = StartCoroutine(BulletTime());
            }
            else if (bulletTimeActive)
            {
                Deactivate();
            }
        }
        else if (bulletTimeActive)
        {
            Deactivate();
        }
    }

    private void ThrowEMP()
    {
        int usedCharge = 0;
        if (AvailableCharge >= usedCharge)
        {
            if (empControllerPrefab != null)
            {
                AvailableCharge -= usedCharge;
                GameObject emp = Instantiate(empControllerPrefab, transform.position, new Quaternion());
                Destroy(emp, 2f);
                OnEMPThrown.Invoke();
            }
        }
    }

    private void StartBoost()
    {
        int usedCharge = 1;
        if (AvailableCharge >= usedCharge)
        {
            AvailableCharge -= usedCharge;

            boostStarted = true;
            StopCoroutine(BoostTimer());
            StartCoroutine(BoostTimer());

            OnBoostStart.Invoke();

            layPurgeTime = 0f;

            if (purgingPlayerBodyPrefab != null)
            {
                purgingPlayerBody = Instantiate(purgingPlayerBodyPrefab, transform);
                purgingPlayerBody.transform.localPosition = Vector3.zero;
            }

            if (boostStartSfx != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(boostStartSfx, 15000, volumeMultiplier: 0.3f);
            }
        }
    }

    private void EndBoost()
    {
        if (boostStarted)
        {
            boostStarted = false;
            StopCoroutine(BoostTimer());

            OnBoostEnd.Invoke();

            if (purgingPlayerBody != null)
            {
                Destroy(purgingPlayerBody);
            }

            if (boostEndSfx != null && SFXController.Instance != null)
            {
                SFXController.Instance.RequestPlay(boostEndSfx, 15000, volumeMultiplier: 0.3f);
            }
        }
    }

    private IEnumerator BoostTimer()
    {
        boostTime = 0;
        while (true)
        {
            if (boostTime >= boostDuration)
            {
                EndBoost();
                break;
            }

            boostTime += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }
}