using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 代码说明：游戏 Actions 定义
 */

namespace Assets.Scripts.Global
{
    /// <summary>
    /// 播放声音
    /// </summary>
    public class PlaySoundAction : Action
    {
        private static ActionHandler staticActionHandler = null;

        public PlaySoundAction() { }
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public PlaySoundAction(string pkg, string name, int type) : base(pkg, name, type)
        {

        }

        public override void Register(ActionHandler handler)
        {
            staticActionHandler = handler;
        }
        public override ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
    }
    /// <summary>
    /// 停止声音
    /// </summary>
    public class StopSoundAction : Action
    {
        private static ActionHandler staticActionHandler = null;

        public StopSoundAction() { }
        /// <summary>
        /// 停止声音
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        public StopSoundAction(string pkg, string name) : base(pkg, name)
        {

        }

        public override void Register(ActionHandler handler)
        {
            staticActionHandler = handler;
        }
        public override ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
    }
    /// <summary>
    /// 设置 SoundManager
    /// </summary>
    public class SetSoundMgrSettingsAction : Action
    {
        private static ActionHandler staticActionHandler = null;

        public SetSoundMgrSettingsAction() { }
        /// <summary>
        /// 设置 SoundManager
        /// </summary>
        /// <param name="mv">音乐音量</param>
        /// <param name="sv">音效音量</param>
        public SetSoundMgrSettingsAction(float mv, float sv) : base(mv, sv)
        {

        }

        public override void Register(ActionHandler handler)
        {
            staticActionHandler = handler;
        }
        public override ActionHandler GetStaticHandler()
        {
            return staticActionHandler;
        }
    }

    /// <summary>
    /// 游戏已经注册的 Actions
    /// </summary>
    public class GameActions
    {
        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="pkg">声音文件所在资源包</param>
        /// <param name="name">声音文件在资源包中的名字</param>
        /// <param name="type">播放参数（[1].背景音乐[2].短声效音乐[3].循环背景音乐[4].循环短声效音乐[5].自定义AudioSource）</param>
        /// <returns></returns>
        public static Action PlaySound(string pkg, string name, int type)
        {
            return new PlaySoundAction(pkg, name, type);
        }
        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="pkg">声音文件所在资源包</param>
        /// <param name="name">声音文件在资源包中的名字</param>
        /// <returns></returns>
        public static Action StopSound(string pkg, string name)
        {
            return new StopSoundAction(pkg, name);
        }
    }
}
