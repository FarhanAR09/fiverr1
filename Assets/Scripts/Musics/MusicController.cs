using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    private static float settingsVolumeMultiplier = 1f;
    public static float SettingsVolumeMultiplier
    {
        get
        {
            return Mathf.Clamp(settingsVolumeMultiplier, 0f, 1f);
        }
        set
        {
            settingsVolumeMultiplier = Mathf.Clamp(value, 0f, 1f);
        }
    }

    public static MusicController Instance;
    private AudioSource src;

    private void Awake()
    {
        //Singleton
        if (Instance == null && Instance != this)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        src = GetComponent<AudioSource>();
    }

    public void SetClip(AudioClip clip)
    {
        src.clip = clip;
    }

    public void Play()
    {
        UpdateVolume();
        src.Play();
    }

    public void Pause()
    {
        src.Pause();
    }

    public void UnPause()
    {
        src.UnPause();
    }

    public void Stop()
    {
        src.Stop();
    }

    public void UpdateVolume()
    {
        src.volume = SettingsVolumeMultiplier;
    }
}