public static class GameConstants
{

    #region System on Chip
    #region Scenes
    public const string SOCMAINMENUSCENE = "SoCMainMenuScene";
    public const string FTCPLAYSCENE = "FtCPlayScene";
    public const string FTCMAINMENUSCENE = "FtCMainMenuScene";
    public const string MLPLAYSCENE = "MLPlayScene";
    public const string MLMAINMENUSCENE = "MLMainMenuScene";
    #endregion
    #endregion

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
    public const string MLLOADGAMEMODE = "mlLoadGameMode";
    /// <summary>
    /// Load this difficulty level
    /// </summary>
    public const string MLLOADLEVEL = "mlLoadLevel";
    public const string MLUNLOCKEDLEVEL = "mlUnlockedLevel";
    public const string MLSELECTCARDTHEMEINDEX = "mlSelectedThemeIndex";

    #region Upgrades
    public const string MLUPGRADEDIFFICULTIES = "mlDifficultiesUpgrade";
    public const string MLUPGRADEFLASH = "mlFlashUpgrade";
    public const string MLUPGRADEFREEZE = "mlFreezeUpgrade";
    #endregion

    #region Assets
    public const string MLCARDTHEMEASSETSPATH = "Data/MLCardThemeAssets";

    public const string MLCARDTHEMEASSETSPATHTEST1 = MLCARDTHEMEASSETSPATH + "/TestAssets1";
    public const string MLCARDTHEMEASSETSPATHTEST2 = MLCARDTHEMEASSETSPATH + "/TestAssets2";
    #endregion

    #region Debug: Feature Switches
    public const string MLFSMAXLEAK = "mlFsMaxLeak";
    #endregion

    #endregion
}
