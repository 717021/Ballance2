using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;

public static class StaticValues
{
    public const string GameVersion = "1.0.0-Ballance2";
    public const string GameBulidVersion = "180411.22.9";

    private static string appFolder = null;
    private static string dataFolder = null;
    private static string coreFolder = null;
    private static string coreLevelsFolder = null;
    private static string modsFolder = null;
    private static string levelsFolder = null;

    public static void StaticValuesInit()
    {
#if UNITY_EDITOR
        appFolder = Application.dataPath.Replace("Assets", "Debug") + "/";
        dataFolder = appFolder;
        coreFolder = appFolder + "Core/";
        coreLevelsFolder = appFolder + "Core/Levels/";
        modsFolder = appFolder + "Mods/";
        levelsFolder = appFolder + "Levels/";
#elif UNITY_STANDALONE_WIN
        appFolder = Application.dataPath.Replace("/Ballance2_Data", "");
        dataFolder = appFolder + "/Ballance2_Data/";
        coreFolder = appFolder + "/Ballance2_Data/Core/";
        coreLevelsFolder = appFolder + "/Ballance2_Data/Core/Levels/";
        modsFolder = appFolder + "/Ballance2_Data/Mods/";
        levelsFolder = appFolder + "/Ballance2_Data/Levels/";
#elif UNITY_ANDROID

         //Application.dataPath;
#else

#endif
    }

    /// <summary>
    /// Ballance2_Data/
    /// </summary>
    public static string DataFolder
    {
        get
        {
            return dataFolder;
        }
    }
    /// <summary>
    /// Ballance2_Data/Core/
    /// </summary>
    public static string CoreFolder
    {
        get
        {
            return coreFolder;
        }
    }
    /// <summary>
    /// Ballance2_Data/Core/levels/
    /// </summary>
    public static string CoreLevelsFolder
    {
        get
        {
            return coreLevelsFolder;
        }
    }
    /// <summary>
    /// Ballance2_Data/Levels/
    /// </summary>
    public static string LevelsFolder
    {
        get
        {
            return levelsFolder;
        }
    }
    /// <summary>
    /// Ballance2_Data/Mods/
    /// </summary>
    public static string ModsFolder
    {
        get
        {
            return modsFolder;
        }
    }
}
