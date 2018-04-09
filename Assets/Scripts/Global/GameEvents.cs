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
}
