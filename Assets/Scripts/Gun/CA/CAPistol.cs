using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CAPistol : MonoBehaviour, ICAGun
{
    //ICAGun
    public bool AbleToShoot { get; private set; }
    public UnityAction<bool> OnShootAbilityStateChanged { get; set; }
    public Vector2 Direction {
        get {
            return new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
        }
    }
    public bool ShootToggled { get; private set; } = false;

    //Cooldown
    private readonly float cooldownDuration = 0.5f;
    private float cooldownTimer = 0f;

    private void Start()
    {
        AbleToShoot = true;
        cooldownTimer = 0f;
    }

    private void FixedUpdate()
    {
        if (!AbleToShoot)
        {
            if (cooldownTimer < cooldownDuration)
            {
                cooldownTimer += Time.fixedDeltaTime;
                print("Cooldown: " + cooldownTimer);
            }
            else
            {
                SetAbleToShootState(true);
            }
        }
    }

    public void Shoot()
    {
        if (!AbleToShoot)
            return;

        print(name + " shot at " + Direction);
        Debug.DrawRay(transform.position, 2f * Direction, Color.red, 1f);
        SetAbleToShootState(false);
        cooldownTimer = 0f;
    }

    private Coroutine shootingLoop;
    public void ToggleShoot(bool on)
    {
        if (on)
        {
            IEnumerator ShootLoop()
            {
                while (true)
                {
                    yield return new WaitUntil(() => AbleToShoot);
                    Shoot();
                }
            }
            if (shootingLoop != null)
                StopCoroutine(shootingLoop);
            shootingLoop = StartCoroutine(ShootLoop());
        }
        else
        {
            if (shootingLoop != null)
                StopCoroutine(shootingLoop);
        }
        ShootToggled = on;
    }

    private void SetAbleToShootState(bool able)
    {
        AbleToShoot = able;
        print(able);
        OnShootAbilityStateChanged?.Invoke(AbleToShoot);
    }
}
