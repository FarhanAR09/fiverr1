using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    private void Awake()
    {
        if (TryGetComponent(out PlayerInput _playerInput))
        {
            playerInput = _playerInput;
        }
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.OnSpaceDown.AddListener(BulletTime);
            playerInput.OnVDown.AddListener(ThrowEMP);
        }
    }

    private void Start()
    {
        GameSpeedManager.TryAddGameSpeedModifier(BULLETTIME, 1f);
    }

    private void FixedUpdate()
    {
        //Debug.Log("Player Charge: " + AvailableCharge);
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.OnSpaceDown.RemoveListener(BulletTime);
            playerInput.OnVDown.RemoveListener(ThrowEMP);
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
    }

    public void FillCharge(int amount)
    {
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
        }

        int usedCharge = 1;
        if (AvailableCharge >= usedCharge)
        {
            if (!bulletTimeActive)
            {
                AvailableCharge -= usedCharge;
                bulletTimeActive = true;
                static IEnumerator BulletTime()
                {
                    float calculatedBulletTimeSpeed = 1;
                    while (true)
                    {
                        calculatedBulletTimeSpeed = Mathf.Lerp(calculatedBulletTimeSpeed, 0.1f, 0.1f);
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
            }
        }
    }
}
