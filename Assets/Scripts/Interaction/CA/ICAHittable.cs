using UnityEngine;

public interface ICAHittable
{
    public GameObject Owner { get; }
    public void Hit();
}
