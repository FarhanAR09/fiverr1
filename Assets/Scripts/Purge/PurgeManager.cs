using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgeManager : MonoBehaviour
{
    private bool isPurging = false;
    private Coroutine purgeCoroutine;

    private void OnEnable()
    {
        GameEvents.OnCacheOverflowed.Add(StartPurge);
        GameEvents.OnLevelUp.Add(ResetPurge);
    }

    private void OnDisable()
    {
        GameEvents.OnCacheOverflowed.Remove(StartPurge);
        GameEvents.OnLevelUp.Remove(ResetPurge);
    }

    private void StartPurge(bool _)
    {
        if (!isPurging)
        {
            isPurging = true;
            if (purgeCoroutine != null)
                StopCoroutine(purgeCoroutine);
            purgeCoroutine = StartCoroutine(Purge());
        }
    }

    private void ResetPurge(bool _)
    {
        if (purgeCoroutine != null)
            StopCoroutine(purgeCoroutine);
        if (isPurging)
        {
            GameEvents.OnPurgeFinished.Publish(true);
        }
        isPurging = false;
        
    }

    private IEnumerator Purge ()
    {
        isPurging = true;
        GameEvents.OnPurgeStarted.Publish(true);
        //Debug.Log("----- Purge Started -----");

        yield return new WaitForSecondsRealtime(10);

        GameEvents.OnPurgeFinished.Publish(true);
        //Debug.Log("----- Purge Finished -----");
    }
}
