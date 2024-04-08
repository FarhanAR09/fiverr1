using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStation : MonoBehaviour
{
    private readonly List<Battery> batteries = new();
    private Battery currentyChargingBattery;

    [SerializeField]
    private GameObject batteryPrefab;
    [SerializeField]
    private ParticleSystem psExplode;

    private void Start()
    {
        if (batteryPrefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                //Battery newBattery = new GameObject("Battery " + i, typeof(Battery)).GetComponent<Battery>();
                GameObject newBattery = Instantiate(batteryPrefab, new Vector3(21f + i * 2.4f, 8.45f), new Quaternion());
                batteries.Add(newBattery.GetComponent<Battery>());
            }
        }
        else Debug.LogWarning("Battery Prefab is null");

        if (psExplode != null)
        {
            psExplode.Emit(50);
        }
    }

    private void FixedUpdate()
    {
        foreach (var battery in batteries)
        {
            battery.AllowCharging = false;
        }
        float highestCharge = float.NegativeInfinity;
        for (int i = 0; i < batteries.Count; i++)
        {
            if (batteries[i].CurrentCharge < 1f && batteries[i].CurrentCharge > highestCharge)
            {
                highestCharge = batteries[i].CurrentCharge;
                foreach (var battery in batteries)
                {
                    battery.AllowCharging = false;
                }
                batteries[i].AllowCharging = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(PlayerInput.GOInstance))
        {
            if (collision.TryGetComponent(out PlayerPowerUpManager powerManager))
            {
                powerManager.FillCharge(GetCharge(powerManager.MaxCharge - powerManager.AvailableCharge));
            }
        }
    }

    /// <summary>
    /// Maximum charge between emptySlots and available battery number
    /// </summary>
    /// <param name="emptySlots"></param>
    /// <returns></returns>
    private int GetCharge(int emptySlots)
    {
        int charge = 0;
        foreach (var battery in batteries)
        {
            if (charge < emptySlots && battery.TryTakeCharge())
            {
                charge++;
            }
        }

        if (psExplode != null && charge > 0)
        {
            psExplode.Emit(50);
        }

        return charge;
    }
}
