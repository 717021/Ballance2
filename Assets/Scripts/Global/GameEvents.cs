using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 代码说明：游戏 Events 定义
 * 
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 游戏退出侦听器
    /// </summary>
    public class OnGameExitLinister : EventLinster
    {
        public OnGameExitLinister(OnGameExitHandler h) : base("GameExiting")
        {
            OnGameExit += h;
        }
        public OnGameExitLinister(OnGameExitHandler h, string difName) : base("GameExiting", difName)
        {
            OnGameExit += h;
        }

        public override void OnEvent(object sender, params object[] par)
        {
            base.OnEvent(sender, par);
            if (OnGameExit != null)
                OnGameExit();
        }

        public delegate void OnGameExitHandler();

        public event OnGameExitHandler OnGameExit;
    }
    //ModLoader 
    /// <summary>
    /// ModLoader 加载完成侦听器
    /// </summary>
    public class OnModLoaderFinishedLinister : EventLinster
    {
        public OnModLoaderFinishedLinister(OnModLoaderFinishedHandler h) : base("ModLoadFinish")
        {
            OnModLoaderFinished += h;
        }

        public override void OnEvent(object sender, params object[] par)
        {
            base.OnEvent(sender, par);
            if (OnModLoaderFinished != null)
                OnModLoaderFinished((bool)par[0], par[1] as string, par[2] as string);
        }

        public delegate void OnModLoaderFinishedHandler(bool success, string info, string errinfo);

        public OnModLoaderFinishedHandler OnModLoaderFinished;
    }
    /// <summary>
    /// UI对话框关闭侦听器
    /// </summary>
    public class OnDialolgClosedLinister : EventLinster
    {
        public OnDialolgClosedLinister(OnDialolgClosedHandler h) : base("UIDialogClosed")
        {
            OnDialolgClosed += h;
        }

        public override void OnEvent(object sender, params object[] par)
        {
            base.OnEvent(sender, par);
            if (OnDialolgClosed != null)
                OnDialolgClosed((int)par[0], (bool)par[1], (bool)par[2]);
        }

        public delegate void OnDialolgClosedHandler(int dlgid, bool clickedok,bool clickedthird);

        public event OnDialolgClosedHandler OnDialolgClosed;
    }

    /// <summary>
    /// 声音管理器设置变化侦听器
    /// </summary>
    public class OnSoundMgrSettingsChangedLinister : EventLinster
    {
        public OnSoundMgrSettingsChangedLinister(OnSoundMgrSettingsChangedHandler h) : base("OnSoundMgrSettingsChanged")
        {
            OnSoundMgrSettingsChanged += h;
        }
        public OnSoundMgrSettingsChangedLinister(OnSoundMgrSettingsChangedHandler h, string difName) : base("OnSoundMgrSettingsChanged", difName)
        {
            OnSoundMgrSettingsChanged += h;
        }

        public override void OnEvent(object sender, params object[] par)
        {
            base.OnEvent(sender, par);
            if (OnSoundMgrSettingsChanged != null)
                OnSoundMgrSettingsChanged();
        }

        public delegate void OnSoundMgrSettingsChangedHandler();

        public event OnSoundMgrSettingsChangedHandler OnSoundMgrSettingsChanged;
    }
}
