using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsPageManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI soundMusicDisplay;

    private float soundVolume = 1f, musicVolume = 1f;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(GameConstants.SOUNDVOLUME))
        {
            soundVolume = PlayerPrefs.GetFloat(GameConstants.SOUNDVOLUME);
        }
        else
        {
            PlayerPrefs.SetFloat(GameConstants.SOUNDVOLUME, soundVolume);
        }

        if (PlayerPrefs.HasKey(GameConstants.MUSICVOLUME))
        {
            musicVolume = PlayerPrefs.GetFloat(GameConstants.MUSICVOLUME);
        }
        else
        {
            PlayerPrefs.SetFloat(GameConstants.MUSICVOLUME, musicVolume);
        }
    }

    private void Start()
    {
        UpdateText();
    }

    public void IncreaseSound()
    {
        UpdateSound(Mathf.Clamp(soundVolume + 0.1f, 0f, 1f));
        UpdateText();
    }

    public void DecreaseSound()
    {
        UpdateSound(Mathf.Clamp(soundVolume - 0.1f, 0f, 1f));
        UpdateText();
    }

    public void IncreaseMusic()
    {
        UpdateMusic(Mathf.Clamp(musicVolume + 0.1f, 0f, 1f));
        UpdateText();
    }

    public void DecreaseMusic()
    {
        UpdateMusic(Mathf.Clamp(musicVolume - 0.1f, 0f, 1f));
        UpdateText();
    }

    private void UpdateText()
    {
        if (soundMusicDisplay != null)
        {
            soundMusicDisplay.SetText($"Sound: {soundVolume * 100f:F0}%\n\n" +
                $"Music: {musicVolume * 100f:F0}%");
        }
    }

    private void UpdateSound(float volume)
    {
        soundVolume = volume;
        SFXController.SettingsVolumeMultiplier = soundVolume;
        PlayerPrefs.SetFloat(GameConstants.SOUNDVOLUME, volume);
    }

    private void UpdateMusic(float volume)
    {
        musicVolume = volume;
        MusicController.SettingsVolumeMultiplier = musicVolume;
        if (MusicController.Instance != null)
        {
            MusicController.Instance.UpdateVolume();
        }
        PlayerPrefs.SetFloat(GameConstants.MUSICVOLUME, volume);
    }
}