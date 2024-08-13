using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    #region Cache Flush Speed Up
    public const string LEVELSPEEDKEY = "LevelTimeSpeedUp";
    public const float LEVELUPSPEEDUP = 0.1f;
    public const float PLAYERSTOPSLOWDOWN = 0.1f;
    #endregion

    #region Settings
    public const string SOUNDVOLUME = "soundVolume";
    public const string MUSICVOLUME = "musicVolume";
    #endregion

    #region Credit
    public const string FTCCREDIT = "ftcCredit";
    public const string MLCREDIT = "mlCredit";
    #endregion

    #region Memory Leak
    /// <summary>
    /// Load this level
    /// </summary>
    public const string MLLOADLEVEL = "mlLoadLevel";
    public const string MLUNLOCKEDLEVEL = "mlUnlockedLevel";
    #endregion
}
