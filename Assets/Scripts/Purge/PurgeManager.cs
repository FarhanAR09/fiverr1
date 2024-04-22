using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgeManager : MonoBehaviour
{
    private bool isPurging = false, levelPurged = false;
    private Coroutine purgeCoroutine;

    [SerializeField]
    private AudioClip purgeWarningSFX;

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
        if (!isPurging && !levelPurged)
        {
            if (purgeCoroutine != null)
                StopCoroutine(purgeCoroutine);
            purgeCoroutine = StartCoroutine(Purge());
        }
    }

    private void ResetPurge(bool _)
    {
        if (purgeCoroutine != null)
        {
            StopCoroutine(purgeCoroutine);
        }
        if (isPurging)
        {
            Debug.Log("Purge resets");
            GameEvents.OnPurgeFinished.Publish(true);
        }
        if (MusicController.instance != null)
        {
            MusicController.instance.UnPause();
        }
        isPurging = false;
        levelPurged = false;
    }

    private IEnumerator Purge ()
    {
        Debug.Log("Purge starts");
        isPurging = true;
        levelPurged = true;
        GameEvents.OnPurgeStarted.Publish(true);
        if (SFXController.Instance != null && purgeWarningSFX != null)
        {
            SFXController.Instance.RequestPlay(purgeWarningSFX, 20000, timePitching: false);
        }

        yield return new WaitForSecondsRealtime(13);

        GameEvents.OnPurgeFinished.Publish(true);
        isPurging = false;
        Debug.Log("Purge finished");
    }
}
