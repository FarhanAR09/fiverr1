using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
        int usedCharge = 1;
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
        Debug.Log("Boost Start");
        boostStarted = true;
        StopCoroutine(BoostTimer());
        StartCoroutine(BoostTimer());

        OnBoostStart.Invoke();
    }

    private void EndBoost()
    {
        if (boostStarted)
        {
            Debug.Log("Boost End");
            boostStarted = false;
            StopCoroutine(BoostTimer());

            OnBoostEnd.Invoke();
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
