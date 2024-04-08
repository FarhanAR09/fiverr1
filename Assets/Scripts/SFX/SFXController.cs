using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class SFXController : MonoBehaviour
{
    public static SFXController Instance { get; private set; }

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
    public bool RequestPlay(AudioClip clip, int priority, bool timePitching = true, float volumeMultiplier = 1)
    {
        foreach (SFXPlayer player in sfxPlayers)
        {
            if (player.RequestPlay(clip, priority, timePitching, volumeMultiplier))
            {
                return true;
            }
        }
        return false;
    }
}
