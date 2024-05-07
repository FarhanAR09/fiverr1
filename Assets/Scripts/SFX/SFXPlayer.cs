using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    public int CurrentPriority { get; private set; } = 0;
    public bool IsPlaying 
    {
        get => audioSource.isPlaying;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool RequestPlay(AudioClip clip, int priority, bool timePitching, float volumeMultiplier, float pitchMultiplier)
    {
        if (audioSource.isPlaying)
        {
            if (CurrentPriority < priority)
            {
                PlayAudio(clip, priority, timePitching, volumeMultiplier, pitchMultiplier);
                return true;
            }
        }
        else
        {
            PlayAudio(clip, priority, timePitching, volumeMultiplier, pitchMultiplier);
            return true;
        }
        return false;
    }

    private void PlayAudio(AudioClip clip, int priority, bool timePitching, float volumeMultiplier, float pitchMultiplier)
    {
        CurrentPriority = priority;
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.volume = Mathf.Clamp(volumeMultiplier, 0f, 1f) * SFXController.SettingsVolumeMultiplier;
        audioSource.pitch = (timePitching ? Time.timeScale : 1) * pitchMultiplier;
        audioSource.Play();
    }
}
