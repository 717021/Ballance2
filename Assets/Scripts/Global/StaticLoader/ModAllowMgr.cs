using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Global.StaticLoader
{
    internal static class ModAllowMgr
    {
        private static string[] gameCorePkgs = new string[] {
#if UNITY_STANDALONE
            "skys_win",
            "musics_win",
            "sounds_win",
#elif UNITY_ANDROID
            "skys_and",
            "musics_and",
            "sounds_and",
#elif UNITY_IOS
            "skys_ios",
            "musics_ios",
            "sounds_ios",
#endif
            "MenuLevel",
            "WorkerPart",
            "",
        };

        /// <summary>
        /// 检查是否是游戏主体mod
        /// </summary>
        /// <param name="pkg">mod名称</param>
        /// <returns></returns>
        public static bool IsGameCore(string pkg)
        {
            return gameCorePkgs.Contains(pkg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <returns></returns>
        public static bool IsAllowLoad(string pkg)
        {
            return false;
        }
    }
}
