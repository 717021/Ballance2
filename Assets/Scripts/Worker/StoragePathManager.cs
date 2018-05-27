using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Worker
{
    /// <summary>
    /// 获取一些目录的路径
    /// </summary>
    public static class StoragePathManager
    {
        private static string appFolder = null;
        private static string dataFolder = null;
        private static string coreFolder = null;
        private static string coreLevelsFolder = null;
        private static string modsFolder = null;
        private static string levelsFolder = null;

        public static void StoragePathInit()
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

        /// <summary>
        /// 获取在Ballance2_Data/Core文件夹里的dll路径
        /// </summary>
        /// <param name="name">dll名字</param>
        /// <returns></returns>
        public static string GetCodeModPathWithName(string name)
        {
            if (name.EndsWith(".dll"))
                return CoreFolder + name;
            else return CoreFolder + name + ".dll";
        }
    }
}
