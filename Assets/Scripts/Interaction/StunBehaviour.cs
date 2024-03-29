using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.Events;

public class StunBehaviour : MonoBehaviour, IStunnable
{
    public UnityEvent OnStunned { get; private set; } = new();
    public UnityEvent OnWakeUp { get; private set; } = new();

    public void SetUp()
    {

    }

    public void Stun(float duration)
    {
        IEnumerator StunTiming()
        {
            OnStunned.Invoke();
            yield return new WaitForSeconds(duration);
            OnWakeUp.Invoke();
        }
        StartCoroutine(StunTiming());
    }
}
