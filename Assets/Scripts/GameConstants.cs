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

    public const string FTCUPGRADEBULLETTIME = "upgradeTimeSlow";
    public const string FTCUPGRADEEMP = "upgradeEMP";
    public const string FTCUPGRADEBOOST = "upgradeBoost";

    #region Settings
    public const string SOUNDVOLUME = "soundVolume";
    public const string MUSICVOLUME = "musicVolume";
    #endregion

    #region Credit Types
    public const string FTCCREDIT = "ftcCredit";
    public const string MLCREDIT = "mlCredit";
    #endregion

    #region Memory Leak
    /// <summary>
    /// Load this level
    /// </summary>
    public const string MLLOADLEVEL = "mlLoadLevel";
    public const string MLUNLOCKEDLEVEL = "mlUnlockedLevel";

    #region Upgrades
    public const string MLUPGRADEDIFFICULTIES = "mlDifficultiesUpgrade";
    public const string MLUPGRADEFLASH = "mlFlashUpgrade";
    public const string MLUPGRADEFREEZE = "mlFreezeUpgrade";
    #endregion

    #endregion
}
