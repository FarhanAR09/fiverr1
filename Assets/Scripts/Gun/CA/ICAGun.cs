using UnityEngine;
using UnityEngine.Events;

public interface ICAGun
{
    public Vector2 Direction { get; }
    public bool ShootToggled { get; }
    public bool AbleToShoot { get; }
    public UnityAction<bool> OnShootAbilityStateChanged { get; set; }
    public void Shoot();
    public void ToggleShoot(bool on);
}
