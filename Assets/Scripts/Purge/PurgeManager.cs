using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgeManager : MonoBehaviour
{
    private bool isPurging = false, levelPurged = false;
    private Coroutine purgeCoroutine;

    [SerializeField]
    private AudioClip purgeWarningSFX;

    [SerializeField]
    private AudioSource audioSource;

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
            GameEvents.OnPurgeFinished.Publish(true);
        }
        if (MusicController.instance != null)
        {
            MusicController.instance.UnPause();
        }
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        isPurging = false;
        levelPurged = false;
    }

    private IEnumerator Purge ()
    {
        isPurging = true;
        levelPurged = true;
        GameEvents.OnPurgeWarning.Publish(true);
        if (audioSource != null && purgeWarningSFX != null)
        {
            //SFXController.Instance.RequestPlay(purgeWarningSFX, 20000, timePitching: false);
            audioSource.clip = purgeWarningSFX;
            audioSource.Play();
        }

        yield return new WaitForSecondsRealtime(3);

        GameEvents.OnPurgeStarted.Publish(true);

        yield return new WaitForSecondsRealtime(10);

        GameEvents.OnPurgeFinished.Publish(true);
        isPurging = false;
    }
}
