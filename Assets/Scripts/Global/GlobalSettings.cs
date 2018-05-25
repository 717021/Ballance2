using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * 说明：游戏设置
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 游戏设置
    /// </summary>
    public static class GlobalSettings
    {
        public static void GlobalSettingsInit()
        {
            int i = PlayerPrefs.GetInt("First");
            if (i == 0)
            {
                CmdLine = 15;
                MusicVolume = 0.5f;
                SoundVolume = 0.5f;
                PlayerPrefs.SetInt("First", 1);
            }
        }

        /// <summary>
        /// 是否调试模式
        /// </summary>
        public static bool Debug
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                int i = PlayerPrefs.GetInt("IsDebug");
                if (i == 1)
                    return true;
                else return false;
#endif
            }
            set
            {
                PlayerPrefs.SetInt("IsDebug", value ? 1 : 0);
            }
        }
        /// <summary>
        /// 是否从Intro开始
        /// </summary>
        public static bool StartInIntro { get; set; }
        /// <summary>
        /// 是否从Intro开始
        /// </summary>
        public static int CmdLine
        {
            get
            {
                int i = PlayerPrefs.GetInt("CmdLine");
                return i;
            }
            set
            {

                PlayerPrefs.SetInt("CmdLine", value);
            }
        }
        /// <summary>
        /// 获取支持的屏幕分辨率
        /// </summary>
        /// <returns></returns>
        public static Resolution[] GetSupportScrrenResolutions()
        {
            return Screen.resolutions;
        }
        /// <summary>
        ///  获取当前的屏幕分辨率
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentScrrenResolution()
        {
            return Screen.currentResolution.width + "x" + Screen.currentResolution.height + " (" + Screen.currentResolution.refreshRate + "Hz)"; 
        }
        /// <summary>
        ///  设置当前的屏幕分辨率
        /// </summary>
        /// <returns></returns>
        public static void SetCurrentScrrenResolution(int i)
        {
            Screen.SetResolution(Screen.resolutions[i].width, Screen.resolutions[i].height, IsFullScreen);
        }
        public static int GetCurrentQualitySettings()
        {
            return QualitySettings.GetQualityLevel();
        }
        public static void SetCurrentQualitySettings(int i)
        {
            QualitySettings.SetQualityLevel(i);
        }
        /// <summary>
        /// 获取设置是否全屏
        /// </summary>
        public static bool IsFullScreen
        {
            get
            {
                return Screen.fullScreen;
            }
            set
            {
                Screen.fullScreen = value;
                PlayerPrefs.SetString("FullScreen", value.ToString());
            }
        }
        /// <summary>
        /// 游戏音乐音量
        /// </summary>
        public static float MusicVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("MusicVolume");
            }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    GlobalMediator.CallAction(new SetSoundMgrSettingsAction(value, -1));
                    PlayerPrefs.SetFloat("MusicVolume", value);
                }
            }
        }
        /// <summary>
        /// 游戏音效音量
        /// </summary>
        public static float SoundVolume
        {
            get
            {
                return PlayerPrefs.GetFloat("SoundVolume");
            }
            set
            {
                if (value >= 0 && value <= 1)
                {
                    GlobalMediator.CallAction(new SetSoundMgrSettingsAction(-1, value));
                    PlayerPrefs.SetFloat("SoundVolume", value);
                }
            }
        }
    }
}
