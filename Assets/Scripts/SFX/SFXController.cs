using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
#endif

/// <summary>
/// To play SFX from everywhere
/// </summary>
[DefaultExecutionOrder(-4999)]
public class SFXController : MonoBehaviour
{
    private static SFXController instance;
    public static SFXController Instance {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    [SerializeField]
    private GameObject SFXPlayerPrefab;
    [SerializeField]
    private int SFXPlayersNumber = 16;
    private readonly List<SFXPlayer> sfxPlayers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        SpawnPlayers();
    }

    private void OnDestroy()
    {
        foreach (SFXPlayer player in sfxPlayers)
        {
            Destroy(player);
        }
    }

    private void SpawnPlayers()
    {
        if (SFXPlayersNumber > 0 && SFXPlayerPrefab != null)
        {
            for (int i = 0; i < SFXPlayersNumber; i++)
            {
                GameObject spawned = Instantiate(SFXPlayerPrefab, transform);
                if (spawned.TryGetComponent(out SFXPlayer _player))
                {
                    sfxPlayers.Add(_player);
                }
            }
        }
    }

    /// <summary>
    /// Use available or lower priority SFX player
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="priority"></param>
    public bool RequestPlay(AudioClip clip, int priority, bool timePitching = true, float volumeMultiplier = 1, float pitchMultiplier = 1)
    {
        foreach (SFXPlayer player in sfxPlayers)
        {
            if (player.RequestPlay(clip, priority, timePitching, volumeMultiplier, pitchMultiplier))
            {
                return true;
            }
        }
        return false;
    }
}
