using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController instance;
    private AudioSource src;

    private void Awake()
    {
        //Singleton
        if (instance == null && instance != this)
        {
            instance = this;
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
}